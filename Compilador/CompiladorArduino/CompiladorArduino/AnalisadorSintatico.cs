using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public class AnalisadorSintatico
    {
        public String Execute(String ArduinoCode)
        {
            LineManager.Instance.setArduinoCode(ArduinoCode);
            TableSymbol.Instance.TableSymbolReset();

            if (LineManager.Instance.ReadLine())
            {
                String ProgCod;

                TableSymbol.CurrentContext = TableSymbol.GlobalContext;

                this.ProgArduino(out ProgCod);

                return ProgCod;
            }

            return "";
        }

        private void ProgArduino(out String ProgCod)
        {
            String LCGCod;
            this.ListaComandosGlobal(out LCGCod);

            ProgCod = LCGCod;
        }

        private void ListaComandosGlobal(out String LCGCod)
        {
            LCGCod = "";
            bool recur_flag = false;

            AnalisadorLexico.Analisar();

            //declaração variaveis
            if (TokenManager.Instance.TokenCode == LexMap.Consts["INTEIRO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["FLOAT"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LONG"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LOGICO"] ||
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
                    String IfCod;
                    this.If(out IfCod);
                    LCGCod += IfCod;
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
                                    //this.Switch();
                                    recur_flag = true;
                                }
                                else

                                    if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
                                    {
                                        String LIdCod = TokenManager.Instance.TokenSymbol;
                                        //atribuição
                                        String AtribCod;
                                        this.Atribuicao(out AtribCod);
                                        LCGCod += AtribCod;
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
                String LCG1Cod;
                this.ListaComandosGlobal(out LCG1Cod);
                LCGCod += LCG1Cod;
            }
        }

        private void ListaComandos(out String LCCod)
        {
            LCCod = "";
            bool recur_flag = false;

            AnalisadorLexico.Analisar();

            //declaração variaveis
            if (TokenManager.Instance.TokenCode == LexMap.Consts["INTEIRO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["FLOAT"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LOGICO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["LONG"])
            {
                LineManager.Instance.ResetToLastPos();
                this.DeclaraVar();
                recur_flag = true;
            } else
            
            //if
            if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
            {

                String IfCod;
                this.If(out IfCod);
                LCCod += IfCod;
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
                //this.Switch();
                recur_flag = true;
            } else
            
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
            {
                String LIdCod = TokenManager.Instance.TokenSymbol;
                //atribuição
                String AtribCod;
                this.Atribuicao(out AtribCod);
                LCCod += AtribCod;
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
                String LC1Cod;
                this.ListaComandos(out LC1Cod);
                LCCod += LC1Cod;
            }
        }

        #region Declaracao

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

                TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, TVTipo, StructureType.Function);

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
                TableSymbol.Instance.Add(DBIdCod, DBTipo);

                return;
            }
            else if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                TableSymbol.Instance.Add(DBIdCod, DBTipo);

                AnalisadorLexico.Analisar();

                if (TokenManager.Instance.TokenCode != LexMap.Consts["ID"])
                {
                    throw new AnalisadorException("Um identificador era esperado.");
                }

                TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, DBTipo);

                this.ListaVar(DBTipo);

                if (TokenManager.Instance.TokenCode != LexMap.Consts["PONTOVIRGULA"])
                {
                    throw new AnalisadorException("O Token ; era esperado.");
                }
            }
            else if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"])
            {
                TableSymbol.Instance.Add(DBIdCod, DBTipo, StructureType.Function);

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

            String LCCod;
            this.ListaComandos(out LCCod);

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

                    String ExpCod, ExpPlace;
                    int ExpTipo;
                    this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);

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

            TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

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

                TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

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

            TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, TVTipo, TableSymbol.CurrentContext);

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
                TokenManager.Instance.TokenCode != LexMap.Consts["LOGICO"] &&
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
                    TableSymbol.Instance.Add(TokenManager.Instance.TokenSymbol, LVTipo, TableSymbol.CurrentContext);

                    this.ListaVar(LVTipo);
                }
                else
                {
                    throw new AnalisadorException("Tipo de variável não pode ser identificado.");
                }
            }
        }

        #endregion Declaracao

        public void Atribuicao(out String AtribCod)
        {
            AtribCod = "";
            String id = TokenManager.Instance.TokenSymbol;
            TableSymbol.Instance.ExistsVar(id);

            String AtribOpCod;
            this.AtribOp(id, out AtribOpCod);
            AtribCod = AtribOpCod;
        }

        public void AtribOp(String AtribOpId, out String AtribOpCod)
        {
            AtribOpCod = "";
            AnalisadorLexico.Analisar();
            String ExpCod, ExpPlace;
            int ExpTipo = 0;
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ATRIBUICAO"])
            {
                this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);
                AtribOpCod = ExpCod + AtribOpId + " = " + ExpPlace + Environment.NewLine;
            }
            else if (TokenManager.Instance.TokenCode == LexMap.Consts["MAIS"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["MENOS"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["MULTIPLICACAO"] ||
                TokenManager.Instance.TokenCode == LexMap.Consts["DIVISAO"])
            {
                String tks = TokenManager.Instance.TokenSymbol;
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ATRIBUICAO"])
                {
                    throw new AnalisadorException("O token = era esperado");
                }
                this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);
                String AtribOpPlace = this.CriaTemp();
                AtribOpCod = ExpCod;
                AtribOpCod += AtribOpPlace + " = " + AtribOpId + " " + tks + " " + ExpPlace + Environment.NewLine;
                AtribOpCod += AtribOpId + " = " + AtribOpPlace + Environment.NewLine;
            }

            int tInt = LexMap.Consts["INTEIRO"];
            int tFloat = LexMap.Consts["FLOAT"];
            int tLog = LexMap.Consts["LOGICO"];
            int idType = TableSymbol.Instance.GetType(AtribOpId);
            if (idType == tLog && ExpTipo != tLog ||
                idType == tInt && ExpTipo == tLog ||
                idType == tFloat && ExpTipo == tLog)
            {
                throw new AnalisadorException("Atribuição com tipo incompatível: " + 
                    LexMap.TokenGetNome(idType) + " = " + LexMap.TokenGetNome(ExpTipo));
            }

        }

        #region ExpressaoAtribuicao

        public void ExpAtrib(out String ECod, out String EPlace, out int ETipo)
        {
            String TCod, TPlace, Rhc, Rhp;
            int TTipo, Rht;
            this.ExpT(out TCod, out TPlace, out TTipo);
            Rhc = TCod;
            Rhp = TPlace;
            Rht = TTipo;

            String Rsc, Rsp;
            int Rst;
            this.ExpR(Rhc, Rhp, Rht, out Rsc, out Rsp, out Rst);
            ECod = Rsc;
            EPlace = Rsp;
            ETipo = Rst;
        }

        public void ExpR(String Rhc, String Rhp, int Rht, out String Rsc, out String Rsp, out int Rst)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["OU"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                String TCod, TPlace, R1hc, R1hp;
                int TTipo, R1ht;
                this.ExpT(out TCod, out TPlace, out TTipo);
                R1hp = this.CriaTemp();
                R1hc = Rhc + TCod + R1hp + " = " + Rhp + " " + opTk + " " + TPlace + Environment.NewLine; //falta operacao
                R1ht = TableSymbol.Instance.CalcType(opType, Rht, TTipo);

                String R1sc, R1sp;
                int R1st;
                this.ExpR(R1hc, R1hp, R1ht, out R1sc, out R1sp, out R1st);
                Rsc = R1sc;
                Rsp = R1sp;
                Rst = R1st;
            }
            else
            {
                Rsc = Rhc;
                Rsp = Rhp;
                Rst = Rht;
            }
        }

        public void ExpT(out String TCod, out String TPlace, out int TTipo)
        {
            String FCod, FPlace, Uhc, Uhp;
            int FTipo, Uht;
            this.ExpF(out FCod, out FPlace, out FTipo);
            Uhc = FCod;
            Uhp = FPlace;
            Uht = FTipo;

            String Usc, Usp;
            int Ust;
            this.ExpU(Uhc, Uhp, Uht, out Usc, out Usp, out Ust);
            TCod = Usc;
            TPlace = Usp;
            TTipo = Ust;

            LineManager.Instance.ResetToLastPos();
        }

        public void ExpU(String Uhc, String Uhp, int Uht, out String Usc, out String Usp, out int Ust)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
            bool is_valid_flag = false;

            if (opType == LexMap.Consts["E"])
            {
                is_valid_flag = true;
            }

            if (is_valid_flag)
            {
                String FCod, FPlace, U1hc, U1hp;
                int FTipo, U1ht;
                this.ExpF(out FCod, out FPlace, out FTipo);
                U1hp = this.CriaTemp();
                U1hc = Uhc + FCod + U1hp + " = " + Uhp + " " + opTk + " " + FPlace + Environment.NewLine; //falta operacao
                U1ht = TableSymbol.Instance.CalcType(opType, Uht, FTipo);

                String U1sc, U1sp;
                int U1st;
                this.ExpU(U1hc, U1hp, U1ht, out U1sc, out U1sp, out U1st);
                Usc = U1sc;
                Usp = U1sp;
                Ust = U1st;
            }
            else
            {
                Usc = Uhc;
                Usp = Uhp;
                Ust = Uht;
            }
        }

        public void ExpF(out String FCod, out String FPlace, out int FTipo)
        {
            String GCod, GPlace;
            int GTipo;
            this.ExpG(out GCod, out GPlace, out GTipo);
            FCod = GCod;
            FPlace = GPlace;
            FTipo = GTipo;
        }

        public void ExpG(out String GCod, out String GPlace, out int GTipo)
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["NAO"])
            {
                String G1Cod, G1Place;
                int G1Tipo;
                this.ExpG(out G1Cod, out G1Place, out G1Tipo);
                GPlace = this.CriaTemp();
                GCod = G1Cod + GPlace + " = !" + G1Place + Environment.NewLine;
                GTipo = G1Tipo;
            }
            else
            {
                LineManager.Instance.ResetToLastPos();

                String HCod, HPlace, Vhc, Vhp;
                int HTipo, Vht;
                this.ExpH(out HCod, out HPlace, out HTipo);
                Vhc = HCod;
                Vhp = HPlace;
                Vht = HTipo;

                String Vsc, Vsp;
                int Vst;
                this.ExpV(Vhc, Vhp, Vht, out Vsc, out Vsp, out Vst);
                GCod = Vsc;
                GPlace = Vsp;
                GTipo = Vst;

                LineManager.Instance.ResetToLastPos();
            }
        }

        public void ExpV(String Vhc, String Vhp, int Vht, out String Vsc, out String Vsp, out int Vst)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
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
                String HCod, HPlace, V1hc, V1hp;
                int HTipo, V1ht;
                this.ExpH(out HCod, out HPlace, out HTipo);
                V1hp = this.CriaTemp();
                V1hc = Vhc + HCod + V1hp + " = " + Vhp + " " + opTk + " " + HPlace + Environment.NewLine;
                V1ht = TableSymbol.Instance.CalcType(opType, Vht, HTipo);

                String V1sc, V1sp;
                int V1st;
                this.ExpV(V1hc, V1hp, V1ht, out V1sc, out V1sp, out V1st);
                Vsc = V1sc;
                Vsp = V1sp;
                Vst = V1st;
            }
            else
            {
                Vsp = Vhp;
                Vsc = Vhc;
                Vst = Vht;
            }
        }

        public void ExpH(out String HCod, out String HPlace, out int HTipo)
        {
            String JCod, JPlace, Xhc, Xhp;
            int JTipo, Xht;
            this.ExpJ(out JCod, out JPlace, out JTipo);
            Xhc = JCod;
            Xhp = JPlace;
            Xht = JTipo;

            String Xsc, Xsp;
            int Xst;
            this.ExpX(Xhc, Xhp, Xht, out Xsc, out Xsp, out Xst);
            HCod = Xsc;
            HPlace = Xsp;
            HTipo = Xst;

            LineManager.Instance.ResetToLastPos();
        }

        public void ExpX(String Xhc, String Xhp, int Xht, out String Xsc, out String Xsp, out int Xst)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
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
                String JCod, JPlace, X1hc, X1hp;
                int JTipo, X1ht;
                this.ExpJ(out JCod, out JPlace, out JTipo);
                X1hp = this.CriaTemp();
                X1hc = Xhc + JCod + X1hp + " = " + Xhp + " " + opTk + " " + JPlace + Environment.NewLine;
                X1ht = TableSymbol.Instance.CalcType(opType, Xht, JTipo);

                String X1sc, X1sp;
                int X1st;
                this.ExpX(X1hc, X1hp, X1ht, out X1sc, out X1sp, out X1st);
                Xsc = X1sc;
                Xsp = X1sp;
                Xst = X1st;
            }
            else
            {
                Xsp = Xhp;
                Xsc = Xhc;
                Xst = Xht;
            }
        }

        public void ExpJ(out String JCod, out String JPlace, out int JTipo)
        {
            String KCod, KPlace, Yhc, Yhp;
            int KTipo, Yht;
            this.ExpK(out KCod, out KPlace, out KTipo);
            Yhc = KCod;
            Yhp = KPlace;
            Yht = KTipo;

            String Ysc, Ysp;
            int Yst;
            this.ExpY(Yhc, Yhp, Yht, out Ysc, out Ysp, out Yst);
            JCod = Ysc;
            JPlace = Ysp;
            JTipo = Yst;

            LineManager.Instance.ResetToLastPos();
        }

        public void ExpY(String Yhc, String Yhp, int Yht, out String Ysc, out String Ysp, out int Yst)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
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
                String KCod, KPlace, Y1hc, Y1hp;
                int KTipo, Y1ht;
                this.ExpK(out KCod, out KPlace, out KTipo);
                Y1hp = this.CriaTemp();
                Y1hc = Yhc + KCod + Y1hp + " = " + Yhp + " " + opTk + " " + KPlace + Environment.NewLine;
                Y1ht = TableSymbol.Instance.CalcType(opType, Yht, KTipo);

                String Y1sc, Y1sp;
                int Y1st;
                this.ExpY(Y1hc, Y1hp, Y1ht, out Y1sc, out Y1sp, out Y1st);
                Ysc = Y1sc;
                Ysp = Y1sp;
                Yst = Y1st;
            }
            else
            {
                Ysp = Yhp;
                Ysc = Yhc;
                Yst = Yht;
            }
        }

        public void ExpK(out String KCod, out String KPlace, out int KTipo)
        {
            KCod = "";
            KPlace = "";
            KTipo = 0;
            AnalisadorLexico.Analisar();
            int tkc = TokenManager.Instance.TokenCode;
            String tk = TokenManager.Instance.TokenSymbol;

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
                        //falta fazer
                        this.Funcao(tk);
                    }
                    else
                    {
                        LineManager.Instance.ResetToLastPos();

                        TableSymbol.Instance.ExistsVar(tk);
                        KPlace = tk;
                        KTipo = TableSymbol.Instance.GetType(tk);
                    }
                }
                else if (tkc == LexMap.Consts["CONSTINTEIRO"])
                {
                    KPlace = this.CriaTemp();
                    KCod = KPlace + " = " + tk + Environment.NewLine;
                    KTipo = LexMap.Consts["INTEIRO"];
                }
                else if (tkc == LexMap.Consts["CONSTFLOAT"])
                {
                    KPlace = this.CriaTemp();
                    KCod = KPlace + " = " + tk + Environment.NewLine;
                    KTipo = LexMap.Consts["FLOAT"];
                }
                else if (tkc == LexMap.Consts["TRUE"] || tkc == LexMap.Consts["FALSE"]) // ver se esta certo
                {
                    KPlace = this.CriaTemp();
                    KCod = KPlace + " = " + tk + Environment.NewLine;
                    KTipo = LexMap.Consts["LOGICO"];
                }

                return;
            }

            if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"])
            {
                String ExpCod, ExpPlace;
                int ExpTipo;
                this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);
                KCod = KCod + ExpCod;
                KPlace = ExpPlace;
                KTipo = ExpTipo;

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

        #endregion ExpressaoAtribuicao

        #region ExpressaoCondicional

        private void ExpCond(out String ExpCod, out String ExpPlace, String ExpTrue, String ExpFalse)
        {
            String Lb = this.GeraLabel();
            String CRhTrue = ExpTrue;
            String CRhFalse = Lb;

            String CTCod, CTPlace, CRhc, CRhp;

            this.ExpCT(out CTCod, out CTPlace, ExpTrue, ExpFalse);
            CRhc = CTCod;
            CRhp = CTPlace;

            String CRsc, CRsp;
            this.ExpCR(CRhc, CRhp, out CRsc, out CRsp, CRhTrue, CRhFalse, Lb);
            ExpCod = CRsc;
            ExpPlace = CRsp;

            if(CRhp == CRsp){
                ExpCod += Lb + ": goto " + ExpFalse + Environment.NewLine;
            }
        }

        private void ExpCR(String CRhc, String CRhp, out String CRsc, out String CRsp, String CRhTrue, String CRhFalse, String CRLb)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;

            if (opType == LexMap.Consts["OU"])
            {
                String Lb = this.GeraLabel();
                String CTTrue = CRhTrue;
                String CTFalse = Lb;
                
                String CTCod, CTPlace, CR1hc, CR1hp;
                this.ExpCT(out CTCod, out CTPlace, CTTrue, CTFalse);
                CR1hp = "";
                CR1hc = CRhc + CRLb + ": " + CTCod;

                String CR1sc, CR1sp;
                String CR1hTrue = CRhTrue, CR1hFalse = CRhFalse;
                this.ExpCR(CR1hc, CR1hp, out CR1sc, out CR1sp, CR1hTrue, CR1hFalse, Lb);
                CRsc = CR1sc;
                CRsp = CR1sp;
            }
            else
            {
                CRsc = CRhc;
                CRsp = CRhp;
            }
        }

        private void ExpCT(out String CTCod, out String CTPlace, String CTTrue, String CTFalse)
        {
            String Lb = this.GeraLabel();
            String CUhTrue = Lb;
            String CUhFalse = CTFalse;

            String CFCod, CFPlace, CUhc, CUhp;

            this.ExpCF(out CFCod, out CFPlace, CTTrue, CTFalse);
            CUhc = CFCod;
            CUhp = CFPlace;

            String CUsc, CUsp;
            this.ExpCU(CUhc, CUhp, out CUsc, out CUsp, CUhTrue, CUhFalse, Lb);
            CTCod = CUsc;
            CTPlace = CUsp;

            if (CUhp == CUsp)
            {
                CTCod += Lb + ": goto " + CTTrue + Environment.NewLine;
            }

            LineManager.Instance.ResetToLastPos();
        }

        private void ExpCU(String CUhc, String CUhp, out String CUsc, out String CUsp, String CUhTrue, String CUhFalse, String CULb)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;

            if (opType == LexMap.Consts["E"])
            {
                String Lb = this.GeraLabel();
                String CFTrue = Lb;
                String CFFalse = CUhFalse;

                String CFCod, CFPlace, CU1hc, CU1hp;
                this.ExpCF(out CFCod, out CFPlace, CFTrue, CFFalse);
                CU1hp = "";
                CU1hc = CUhc + CULb + ": " + CFCod;

                String CU1sc, CU1sp;
                String CU1hTrue = CUhTrue, CU1hFalse = CUhFalse;
                this.ExpCR(CU1hc, CU1hp, out CU1sc, out CU1sp, CU1hTrue, CU1hFalse, Lb);
                CUsc = CU1sc;
                CUsp = CU1sp;
            }
            else
            {
                CUsc = CUhc;
                CUsp = CUhp;
            }
        }

        private void ExpCF(out String CFCod, out String CFPlace, String CFTrue, String CFFalse)
        {
            String CGCod, CGPlace;
            this.ExpCG(out CGCod, out CGPlace, CFTrue, CFFalse);
            CFCod = CGCod;
            CFPlace = CGPlace;
        }

        private void ExpCG(out String CGCod, out String CGPlace, String CGTrue, String CGFalse)
        {
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["NAO"])
            {
                String CG1Cod, CG1Place;
                this.ExpCG(out CG1Cod, out CG1Place, CGFalse, CGTrue);
                CGPlace = this.CriaTemp();
                CGCod = CG1Cod + CGPlace + " = !" + CG1Place + Environment.NewLine;
            }
            else
            {
                LineManager.Instance.ResetToLastPos();

                String CH1Cod, CH1Place;
                this.ExpCH(out CH1Cod, out CH1Place, CGTrue, CGFalse);
                CGCod = CH1Cod;
                CGPlace = CH1Place;

                AnalisadorLexico.Analisar();
                Int32 opType = TokenManager.Instance.TokenCode;
                String opTk = TokenManager.Instance.TokenSymbol;
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
                    String CH2Cod, CH2Place;
                    this.ExpCH(out CH2Cod, out CH2Place, null, null);
                    CGPlace = this.CriaTemp();
                    CGCod += CH2Cod + CGPlace + " = " + CH1Place + " " + opTk + " " + CH2Place + Environment.NewLine
                        + "if " + CGPlace + " != 0 goto " + CGTrue + Environment.NewLine
                        + "goto " + CGFalse + Environment.NewLine;
                }

                LineManager.Instance.ResetToLastPos();
            }
        }

        private void ExpCH(out String CHCod, out String CHPlace, String CHTrue, String CHFalse)
        {
            String CJCod, CJPlace, CXhc, CXhp;
            this.ExpCJ(out CJCod, out CJPlace, CHTrue, CHFalse);
            CXhc = CJCod;
            CXhp = CJPlace;
            
            String CXsc, CXsp;
            this.ExpCX(CXhc, CXhp, out CXsc, out CXsp);
            CHCod = CXsc;
            CHPlace = CXsp;

            if(CHTrue != null && CXhp != CXsp){
                CHCod += "if " + CHPlace + " != 0 goto " + CHTrue + Environment.NewLine
                    + "goto " + CHFalse + Environment.NewLine;
            }

            LineManager.Instance.ResetToLastPos();
        }

        public void ExpCX(String CXhc, String CXhp, out String CXsc, out String CXsp)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
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
                String CJCod, CJPlace, CX1hc, CX1hp;
                this.ExpCJ(out CJCod, out CJPlace, null, null);
                CX1hp = this.CriaTemp();
                CX1hc = CXhc + CJCod + CX1hp + " = " + CXhp + " " + opTk + " " + CJPlace + Environment.NewLine;
                
                String CX1sc, CX1sp;
                this.ExpCX(CX1hc, CX1hp, out CX1sc, out CX1sp);
                CXsc = CX1sc;
                CXsp = CX1sp;
            }
            else
            {
                CXsp = CXhp;
                CXsc = CXhc;
            }
        }

        public void ExpCJ(out String CJCod, out String CJPlace, String CJTrue, String CJFalse)
        {
            String CKCod, CKPlace, CYhc, CYhp;
            this.ExpCK(out CKCod, out CKPlace, CJTrue, CJFalse);
            CYhc = CKCod;
            CYhp = CKPlace;

            String CYsc, CYsp;
            this.ExpCY(CYhc, CYhp, out CYsc, out CYsp);
            CJCod = CYsc;
            CJPlace = CYsp;

            if (CJTrue != null && CYhp != CYsp)
            {
                CJCod += "if " + CJPlace + " != 0 goto " + CJTrue + Environment.NewLine
                    + "goto " + CJFalse + Environment.NewLine;
            }

            LineManager.Instance.ResetToLastPos();
        }

        public void ExpCY(String CYhc, String CYhp, out String CYsc, out String CYsp)
        {
            AnalisadorLexico.Analisar();
            Int32 opType = TokenManager.Instance.TokenCode;
            String opTk = TokenManager.Instance.TokenSymbol;
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
                String CKCod, CKPlace, CY1hc, CY1hp;
                this.ExpCK(out CKCod, out CKPlace, null, null);
                CY1hp = this.CriaTemp();
                CY1hc = CYhc + CKCod + CY1hp + " = " + CYhp + " " + opTk + " " + CKPlace + Environment.NewLine;
                
                String CY1sc, CY1sp;
                this.ExpCY(CY1hc, CY1hp, out CY1sc, out CY1sp);
                CYsc = CY1sc;
                CYsp = CY1sp;
            }
            else
            {
                CYsp = CYhp;
                CYsc = CYhc;
            }
        }

        public void ExpCK(out String CKCod, out String CKPlace, String CKTrue, String CKFalse)
        {
            CKCod = "";
            CKPlace = "";
            AnalisadorLexico.Analisar();
            int tkc = TokenManager.Instance.TokenCode;
            String tk = TokenManager.Instance.TokenSymbol;

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
                        //falta fazer
                        this.Funcao(tk);
                    }
                    else
                    {
                        LineManager.Instance.ResetToLastPos();

                        TableSymbol.Instance.ExistsVar(tk);

                        CKPlace = tk;
                        if (TableSymbol.Instance.GetType(tk) == LexMap.Consts["LOGICO"])
                        {
                            CKCod = "if " + CKPlace + " == true goto " + CKTrue + Environment.NewLine
                                + "goto " + CKFalse + Environment.NewLine;
                        }
                    }
                }
                else if (tkc == LexMap.Consts["CONSTINTEIRO"])
                {
                    CKPlace = this.CriaTemp();
                    CKCod = CKPlace + " = " + tk + Environment.NewLine;
                    if(CKTrue != null){
                        CKCod += "if " + CKPlace + " != 0 goto " + CKTrue + Environment.NewLine
                                + "goto " + CKFalse + Environment.NewLine;
                    }
                }
                else if (tkc == LexMap.Consts["CONSTFLOAT"])
                {
                    CKPlace = this.CriaTemp();
                    CKCod = CKPlace + " = " + tk + Environment.NewLine;
                    if (CKTrue != null)
                    {
                        CKCod += "if " + CKPlace + " != 0 goto " + CKTrue + Environment.NewLine
                                + "goto " + CKFalse + Environment.NewLine;
                    }
                }
                else if (tkc == LexMap.Consts["TRUE"] || tkc == LexMap.Consts["FALSE"])
                {
                    CKPlace = this.CriaTemp();
                    CKCod = CKPlace + " = " + tk + Environment.NewLine;
                    if (tkc == LexMap.Consts["TRUE"])
                    {
                        CKCod += "goto " + CKTrue + Environment.NewLine;
                    }
                    else if (tkc == LexMap.Consts["FALSE"])
                    {
                        CKCod += "goto " + CKFalse + Environment.NewLine;
                    }
                }

                return;
            }

            if (TokenManager.Instance.TokenCode == LexMap.Consts["ABREPAR"])
            {
                String ExpCod, ExpPlace;
                this.ExpCond(out ExpCod, out ExpPlace, CKTrue, CKFalse);
                CKCod = CKCod + ExpCod;
                CKPlace = ExpPlace;

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException(
                       String.Format("Era esperado o token {0}", LexMap.TokenGetNome(LexMap.Consts["FECHAPAR"]))
                    );
                }

                return;
            }

            throw new AnalisadorException("O valor da expressão (CK) não é válido");
        }

        #endregion ExpressaoCondicional

        private void If(out String IfCod)
        {
            IfCod = "";
            if (TokenManager.Instance.TokenCode == LexMap.Consts["IF"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                String LIf = this.GeraLabel();
                String LElse = this.GeraLabel();
                String LFim = this.GeraLabel();

                String ExpCod, ExpPlace;
                this.ExpCond(out ExpCod, out ExpPlace, LIf, LElse);
                IfCod += ExpCod;
                
                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }
                
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                IfCod += LIf + ": ";

                String LCCod;
                this.ListaComandos(out LCCod);
                IfCod += LCCod;

                IfCod += "goto " + LFim + Environment.NewLine;

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHACHAVES"])
                {
                    throw new AnalisadorException("O token } era esperado");
                }

                String IfEndCod;
                this.IfEnd(out IfEndCod, LElse, LFim);
                IfCod += IfEndCod;
            }
        }

        private void IfEnd(out String IfEndCod, String LElse, String LFim)
        {
            IfEndCod = "";
            AnalisadorLexico.Analisar();
            if (TokenManager.Instance.TokenCode == LexMap.Consts["ELSE"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode == LexMap.Consts["ABRECHAVES"])
                {
                    IfEndCod += LElse + ": ";
                    String LCCod;
                    this.ListaComandos(out LCCod);
                    IfEndCod += LCCod;

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
                    IfEndCod += LElse + ": ";

                    String IfCod;
                    this.If(out IfCod);
                    IfEndCod += IfCod;

                    IfEndCod += "goto " + LFim + Environment.NewLine;
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

        #region loops

        private void While()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["WHILE"])
            {
                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABREPAR"])
                {
                    throw new AnalisadorException("O token ( era esperado");
                }

                String LWhile = this.GeraLabel();
                String LFim = this.GeraLabel();

                String ExpCod, ExpPlace;
                this.ExpCond(out ExpCod, out ExpPlace, LWhile, LFim);

                if (TokenManager.Instance.TokenCode != LexMap.Consts["FECHAPAR"])
                {
                    throw new AnalisadorException("O token ) era esperado");
                }

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                String LCCod;
                this.ListaComandos(out LCCod);

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
                String LDoWhile = this.GeraLabel();
                String LFim = this.GeraLabel();

                AnalisadorLexico.Analisar();
                if (TokenManager.Instance.TokenCode != LexMap.Consts["ABRECHAVES"])
                {
                    throw new AnalisadorException("O token { era esperado");
                }

                String LCCod;
                this.ListaComandos(out LCCod);

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

                String ExpCod, ExpPlace;
                this.ExpCond(out ExpCod, out ExpPlace, LDoWhile, LFim);

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

                String LFor = this.GeraLabel();
                String LFim = this.GeraLabel();

                String ExpCod, ExpPlace;
                this.ExpCond(out ExpCod, out ExpPlace, LFor, LFim);

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

                String LCCod;
                this.ListaComandos(out LCCod);

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
            String AtribCod;
            this.Atribuicao(out AtribCod);

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

                String AtribCod;
                this.Atribuicao(out AtribCod);

                this.ListaAtribA();
            }
        }

        #endregion loops

        private void Funcao(String FIdCod)
        {
            //if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
            //{
            //    AnalisadorLexico.Analisar();
                TableSymbol.Instance.ExistsFunction(FIdCod);  

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
            String ExpCod, ExpPlace;
            int ExpTipo;
            this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);
            
            this.ListaParamRec();
        }

        private void ListaParamRec()
        {
            if (TokenManager.Instance.TokenCode == LexMap.Consts["VIRGULA"])
            {
                String ExpCod, ExpPlace;
                int ExpTipo;
                this.ExpAtrib(out ExpCod, out ExpPlace, out ExpTipo);

                this.ListaParamRec();
            }
        }

        private int nTemp;
        private String CriaTemp()
        {
            String res = "T" + this.nTemp;
            this.nTemp++;
            return res;
        }

        private int nLabel;
        private String GeraLabel()
        {
            String res = "L" + this.nLabel;
            this.nLabel++;
            return res;
        }

    }
}