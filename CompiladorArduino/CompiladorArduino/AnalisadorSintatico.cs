using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public class AnalisadorSintatico
    {
        public String Execute()
        {
            if (LineManager.Instance.ReadLine())
            {
                TableSymbol.CurrentContext = TableSymbol.GlobalContext;

                this.ProgArduino();
            }

            return "";
        }

        private void ProgArduino()
        {
            this.ListaComandosGlobal();
        }

        private void ListaComandosGlobal()
        {
            bool recur_flag = false;

            AnalisadorLexico.Analisar();

            //declaração variaveis
            if (TokenManager.Instance.TokenCode == LexMap.Consts["INTEIRO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["FLOAT"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["BYTE"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LONG"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["VOID"])
            {
                LineManager.Instance.ResetToLastPos();
                this.Declaracao();
                recur_flag = true;
            }
            else

                //if
                if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
                {
                    this.If();
                    recur_flag = true;
                }
                else

                    //while
                    if (TokenManager.Instance.TokenCode == LexMap.Consts["WHILE"])
                    {
                        this.While();
                        recur_flag = true;
                    }
                    else

                        //do while
                        if (TokenManager.Instance.TokenCode == LexMap.Consts["DO"])
                        {
                            this.DoWhile();
                            recur_flag = true;
                        }
                        else

                            //for
                            if (TokenManager.Instance.TokenCode == LexMap.Consts["FOR"])
                            {
                                this.For();
                                recur_flag = true;
                            }
                            else

                                //switch
                                if (TokenManager.Instance.TokenCode == LexMap.Consts["SWITCH"])
                                {
                                    this.Switch();
                                    recur_flag = true;
                                }
                                else

                                    if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
                                    {
                                        String LIdCod = TokenManager.Instance.TokenSymbol;
                                        //atribuição
                                        this.Atribuicao();
                                        recur_flag = true;

                                        //funcao
                                        if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"] || TokenManager.Instance.TokenCode == LexMap.Consts["PONTO"])
                                        {
                                            //LineManager.Instance.ResetToLastPos(); // nao retrocede por causa da Atribuicao
                                            this.Funcao(LIdCod);
                                            AnalisadorLexico.Analisar();
                                        }

                                        if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                                        {
                                            throw new AnalisadorException("O Token ; era esperado");
                                        }
                                    }

            //recursão
            if (recur_flag == true)
            {
                this.ListaComandosGlobal();
            }
        }


        private void ListaComandos()
        {
            bool recur_flag = false;

            AnalisadorLexico.Analisar();

            //declaração variaveis
            if (TokenManager.Instance.TokenCode == LexMap.Consts["INTEIRO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["FLOAT"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["BYTE"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LONG"])
            {
                LineManager.Instance.ResetToLastPos();
                this.DeclaraVar();
                recur_flag = true;
            } else
            
            //if
            if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
            {
                this.If();
                recur_flag = true;
            } else
            
            //while
            if (TokenManager.Instance.TokenCode == LexMap.Consts["WHILE"])
            {
                this.While();
                recur_flag = true;
            } else
            
            //do while
            if (TokenManager.Instance.TokenCode == LexMap.Consts["DO"])
            {
                this.DoWhile();
                recur_flag = true;
            } else 

            //for
            if (TokenManager.Instance.TokenCode == LexMap.Consts["FOR"])
            {
                this.For();
                recur_flag = true;
            } else

            //switch
            if (TokenManager.Instance.TokenCode == LexMap.Consts["SWITCH"])
            {
                this.Switch();
                recur_flag = true;
            } else
            
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
            {
                String LIdCod = TokenManager.Instance.TokenSymbol;
                //atribuição
                this.Atribuicao();
                recur_flag = true;
                
                //funcao
                if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"] || TokenManager.Instance.TokenCode == LexMap.Consts["PONTO"])
                {
                    //LineManager.Instance.ResetToLastPos(); // nao retrocede por causa da Atribuicao
                    this.Funcao(LIdCod);
                    AnalisadorLexico.Analisar();
                }
                
                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O Token ; era esperado");
                }
            }

            //recursão
            if (recur_flag == true)
            {
                this.ListaComandos();
            }
        }

        /*
        Declaracao -> TipoVar id DecB | void id ( DecC
        DecB -> ; | , id ListaVar ; | ( DecC
        DecC -> ListaParm) {ListaComandos}
        */
        private void Declaracao()
        {
            int gramatica = 0;
            int TVTipo = 0;
            try
            {
                this.TipoVar(out TVTipo);
            }
            catch (AnalisadorException) //o tipo ainda pode ser void
            {
                gramatica = 1;
            }

            if (gramatica == 0)
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("Um identificador era esperado.");
                }

                String DIdCod = TokenManager.Instance.TokenSymbol;
                this.DecB(TVTipo, DIdCod);
            }
            else
            {
                TVTipo = TokenManager.Instance.TokenCode;
                if (TokenManager.Instance.TokenCode != LexMap.Consts["VOID"])
                {
                    throw new AnalisadorException("Tipo de variável não pode ser identificado.");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("Um identificador era esperado.");
                }

                TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, TVTipo, StructureType.Function);

                TableSymbol.CurrentContext = TokenManager.Instance.TokenSymbol;

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado.");
                }

                this.DecC();
            }
        }

        private void DecB(int DBTipo, String DBIdCod)
        {
            AnalisadorLexico.Analisar();

            if (TokenManager.Instance.TokenCode == LexMap.Consts["PONTOVIRGULA"])
            {
                TableSymbol.getInstance().Add(DBIdCod, DBTipo);

                return;
            }
            else if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                TableSymbol.getInstance().Add(DBIdCod, DBTipo);

                AnalisadorLexico.Analisar();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("Um identificador era esperado.");
                }

                TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, DBTipo);

                this.ListaVar(DBTipo);

                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O Token ; era esperado.");
                }
            }
            else if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"])
            {
                TableSymbol.getInstance().Add(DBIdCod, DBTipo, StructureType.Function);

                TableSymbol.CurrentContext = DBIdCod;

                this.DecC();
            }
            else
            {
                throw new AnalisadorException("O token ; , ( era esperado.");
            }
        }

        private void DecC()
        {
            //AnalisadorLexico.Analisar();

            this.ListaDecParm();

            if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
            {
                throw new AnalisadorException("O token ) era esperado.");
            }

            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
            {
                throw new AnalisadorException("O token { era esperado.");
            }

            this.ListaComandos();

            this.Retorno();

            //AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
            {
                throw new AnalisadorException("O token } era esperado.");
            }

            TableSymbol.CurrentContext = TableSymbol.GlobalContext;
        }

        private void Retorno()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["RETURN"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode == LexMap.Consts["PONTOVIRGULA"])
                {
                    AnalisadorLexico.Analisar();
                    return;
                }
                else
                {
                    LineManager.Instance.ResetToLastPos();
                    this.Exp();
                    if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                    {
                        throw new AnalisadorException("O token ; era esperado.");
                    }
                    AnalisadorLexico.Analisar();
                }
            }
        }
        
        /*
            ListaDecParm -> TipoVar id ListaDecParmB | {}
            ListaDecParmB -> , TipoVar id ListaDecParmB | {}
         */
        private void ListaDecParm()
        {
            int TVTipo = 0;
            try
            {
                this.TipoVar(out TVTipo);
            }
            catch (AnalisadorException) {
                //vazio
                return;
            }

            //se TipoVar não causou uma exception, então estou dando parse nos parametros

            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
            {
                throw new AnalisadorException("Um identificador era esperado");
            }

            TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

            this.ListaDecParmB();
        }

        private void ListaDecParmB()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                int TVTipo = 0;
                this.TipoVar(out TVTipo);

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("Um identificador era esperado");
                }

                TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

                this.ListaDecParmB();
            }
        }

        private void DeclaraVar()
        {
            int TVTipo = 0;
            this.TipoVar(out TVTipo);

            AnalisadorLexico.Analisar();

            if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
            {
                throw new AnalisadorException("Um identificador era esperado");
            }

            TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

            this.ListaVar(TVTipo);

            if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
            {
                throw new AnalisadorException("O Token ; era esperado.");
            }
        }

        public void TipoVar(out int TVTipo)
        {
            AnalisadorLexico.Analisar();

            if (TokenManager.Instance.TokenCode != LexMap.Consts["INTEIRO"] &&
                TokenManager.Instance.TokenCode != LexMap.Consts["FLOAT"] &&
                TokenManager.Instance.TokenCode != LexMap.Consts["BYTE"] &&
                TokenManager.Instance.TokenCode != LexMap.Consts["LONG"])
            {
                throw new AnalisadorException("Tipo de variável não pode ser identificado.");
            }

            TVTipo = TokenManager.Instance.TokenCode;
        }

        public void ListaVar(int LVTipo)
        {
            AnalisadorLexico.Analisar();

            if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                AnalisadorLexico.Analisar();

                if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
                {
                    TableSymbol.getInstance().Add(TokenManager.Instance.TokenSymbol, LVTipo, TableSymbol.CurrentContext);

                    this.ListaVar(LVTipo);
                }
                else
                {
                    throw new AnalisadorException("Tipo de variável não pode ser identificado.");
                }
            }
        }

        #region Expressao

        public void Exp()
        {
            this.ExpT();

            this.ExpR();
        }

        public void ExpR()
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["OU"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                this.ExpT();

                this.ExpR();
            }
        }

        public void ExpT()
        {
            this.ExpF();

            this.ExpU();
            LineManager.Instance.ResetToLastPos();
        }

        public void ExpF()
        {
            this.ExpG();
        }

        public void ExpU()
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["E"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                this.ExpF();

                this.ExpU();
            }
        }

        public void ExpG()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["NAO"])
            {
                this.ExpG();
            }
            else
            {
                //ACHO
                LineManager.Instance.ResetToLastPos();

                this.ExpH();


                this.ExpV();

                LineManager.Instance.ResetToLastPos();
            }
        }

        public void ExpV()
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["MAIOR"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["IGUAL"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["MENOR"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["MAIORIGUAL"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["DIFERENTE"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["MENORIGUAL"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                this.ExpH();

                this.ExpV();
            }
        }

        public void ExpH()
        {
            this.ExpJ();

            this.ExpX();
            LineManager.Instance.ResetToLastPos();
        }

        public void ExpJ()
        {
            this.ExpK();

            this.ExpY();
            LineManager.Instance.ResetToLastPos();
        }

        public void ExpX()
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["MAIS"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["MENOS"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                this.ExpJ();

                this.ExpX();
            }
        }

        public void ExpY()
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["MULTIPLICACAO"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["DIVISAO"])
            {
                is_valid_flag = true;
            }

            if (opType == LexMap.Consts["MODC"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                this.ExpK();

                this.ExpY();
            }
        }

        public void ExpK()
        {
            AnalisadorLexico.Analisar();
            int tkc = TokenManager.Instance.TokenCode;
            String EIdCod = TokenManager.Instance.TokenSymbol;

            if (tkc == LexMap.Consts["CONSTINTEIRO"] ||
                tkc == LexMap.Consts["CONSTFLOAT"] ||
                tkc == LexMap.Consts["CONSTFLOATPONTO"] ||
                tkc == LexMap.Consts["CONSTFLOATPONTONUM"] ||
                tkc == LexMap.Consts["CONSTFLOATNUME"] ||
                tkc == LexMap.Consts["CONSTFLOATNUMPONTO"] ||
                tkc == LexMap.Consts["CONSTFLOATE"] ||
                tkc == LexMap.Consts["TRUE"] ||
                tkc == LexMap.Consts["FALSE"] ||
                tkc == LexMap.Consts["HIGH"] ||
                tkc == LexMap.Consts["LOW"] ||
                tkc == LexMap.Consts["INPUT"] ||
                tkc == LexMap.Consts["OUTPUT"] ||
                tkc == LexMap.Consts["ID"])
            {
                if (tkc == LexMap.Consts["ID"])
                {
                    AnalisadorLexico.Analisar();
                    if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"] || TokenManager.Instance.TokenCode == LexMap.Consts["PONTO"])
                    {
                        this.Funcao(EIdCod);
                    }
                    else
                    {
                        LineManager.Instance.ResetToLastPos();

                        TableSymbol.getInstance().ExistsVar(EIdCod);
                    }
                }

                return;
            }

            if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"])
            {
                this.Exp();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException(
                       String.Format("Era esperado o token {0}", LexMap.TokenGetNome(LexMap.Consts["FECHAPAR"]))
                    );
                }

                return;
            }

            throw new AnalisadorException("O valor da expressão (K) não é válido");
        }

        #endregion Expressao

        public void Atribuicao()
        {
            TableSymbol.getInstance().ExistsVar(TokenManager.Instance.TokenSymbol);

            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ATRIBUICAO"])
            {
                this.Exp();
            }
        }

        private void If()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                this.Exp();
                
                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }
                
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }
                
                this.ListaComandos();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }

                this.IfEnd();
            }
        }

        private void IfEnd()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ELSE"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode == LexMap.Consts["ABRECHAVES"])
                {
                    this.ListaComandos();

                    if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                    {
                        throw new AnalisadorException("O token } era esperado");
                    }
                }
                else if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
                {
                    // quebra de linha entre else e if da erro: 
                    // if(a){ if(a){}else 
                    // if(1){} }
                    // no case tbm:
                    // switch(1){
	                // case 
                    // }
                    this.If();
                }
                else
                {
                    throw new AnalisadorException("O token ( ou if era esperado");
                }
            }
            else
            {
                LineManager.Instance.ResetToLastPos();
            }
        }

        private void While()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["WHILE"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                this.Exp();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaComandos();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }
            }
        }

        private void DoWhile()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["DO"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaComandos();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["WHILE"])
                {
                    throw new AnalisadorException("O token while era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                this.Exp();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O token ; era esperado");
                }
            }
        }

        private void For()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["FOR"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                this.ListaAtrib();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O token ; era esperado");
                }

                this.Exp();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O token ; era esperado");
                }

                // falta o i++
                this.ListaAtrib();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaComandos();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }
            }
        }

        private void ListaAtrib()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
            {
                throw new AnalisadorException("O token identificador era esperado");
            }
            this.Atribuicao();

            this.ListaAtribA();
        }

        private void ListaAtribA()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("O token identificador era esperado");
                }

                this.Atribuicao();

                this.ListaAtribA();
            }
        }

        private void Switch()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["SWITCH"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                this.Exp();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaCase();

                this.SwitchDefault();

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }
            }
        }

        private void ListaCase()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["CASE"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["CONSTINTEIRO"])
                {
                    throw new AnalisadorException("O token constante inteiro era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["DOISPONTOS"])
                {
                    throw new AnalisadorException("O token : era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaComandos();

                this.CaseEnd();

                this.ListaCase();
            }
            else
            {
                LineManager.Instance.ResetToLastPos();
            }
        }

        private void CaseEnd()
        {
            if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
            {
                throw new AnalisadorException("O token } era esperado");
            }

            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["BREAK"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O token ; era esperado");
                }
            }
            else
            {
                LineManager.Instance.ResetToLastPos();
            }
        }

        private void SwitchDefault()
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["DEFAULT"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["DOISPONTOS"])
                {
                    throw new AnalisadorException("O token : era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                this.ListaComandos();

                this.CaseEnd();
            }
            else
            {
                LineManager.Instance.ResetToLastPos();
            }
        }

        private void Funcao(String FIdCod)
        {
            //if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
            //{
            //    AnalisadorLexico.Analisar();
                TableSymbol.getInstance().ExistsFunction(FIdCod);

                if (TokenManager.Instance.TokenCode == LexMap.Consts["PONTO"])
                {
                    AnalisadorLexico.Analisar();
                    if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                    {
                        throw new AnalisadorException("O token id era esperado, id.id()");
                    }

                    AnalisadorLexico.Analisar();
                }

                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                AnalisadorLexico.Analisar();
                //se for sem parametro
                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    LineManager.Instance.ResetToLastPos();
                    this.ListaParam();
                }

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }
            //}
        }
        
        private void ListaParam()
        {
            this.Exp();
            
            this.ListaParamRec();
        }

        private void ListaParamRec()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                this.Exp();

                this.ListaParamRec();
            }
        }
    }
}