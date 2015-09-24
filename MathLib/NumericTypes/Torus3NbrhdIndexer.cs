namespace MathLib.NumericTypes
{
    public static class Torus3NbrhdExt
    {

        public static Torus3NbrhdIndexer ToTorus3Nbrs(this int index, int width, int height, int offset=0)
        {

            var R = index / width;
            var C = index % width;

            var rowUuu = offset +  ((R - 3 + height) % height) * width;
            var rowUu  = offset +  ((R - 2 + height) % height) * width;
            var rowU   = offset +  ((R - 1 + height) % height) * width;
            var rowC =   offset +  (R % height) * width;
            var rowL   = offset +  ((R + 1) % height) * width;
            var rowLl  = offset +  ((R + 2) % height) * width;
            var rowLll = offset +  ((R + 3) % height) * width;

            var colFff = (C - 3 + width) % width;
            var colFf  = (C - 2 + width) % width;
            var colF   = (C - 1 + width) % width;
            var colC   = C;
            var colR   = (C + 1) % width;
            var colRr  = (C + 2) % width;
            var colRrr = (C + 3) % width;


            return new Torus3NbrhdIndexer
                (
                    uuuFff: rowUuu + colFff,
                    uuuFf:  rowUuu + colFf,
                    uuuF:   rowUuu + colF,
                    uuuC:   rowUuu + colC,
                    uuuR:   rowUuu + colR,
                    uuuRr:  rowUuu + colRr,
                    uuuRrr: rowUuu + colRrr,
                    uuFff:  rowUu  + colFff,
                    uuFf:   rowUu  + colFf,
                    uuF:    rowUu  + colF,
                    uuC:    rowUu  + colC,
                    uuR:    rowUu  + colR,
                    uuRr:   rowUu  + colRr,
                    uuRrr:  rowUu  + colRrr,
                    uFff:   rowU   + colFff,
                    uFf:    rowU   + colFf,
                    uf:     rowU   + colF,
                    uc:     rowU   + colC,
                    ur:     rowU   + colR,
                    uRr:    rowU   + colRr,
                    uRrr:   rowU   + colRrr,
                    cFff:   rowC   + colFff,
                    cFf:    rowC   + colFf,
                    cf:     rowC   + colF,
                    cc:     rowC   + colC,
                    cr:     rowC   + colR,
                    cRr:    rowC   + colRr,
                    cRrr:   rowC   + colRrr,
                    lFff:   rowL   + colFff,
                    lFf:    rowL   + colFf,
                    lf:     rowL   + colF,
                    lc:     rowL   + colC,
                    lr:     rowL   + colR,
                    lRr:    rowL   + colRr,
                    lRrr:   rowL   + colRrr,
                    llFff:  rowLl  + colFff,
                    llFf:   rowLl  + colFf,
                    llF:    rowLl  + colF,
                    llC:    rowLl  + colC,
                    llR:    rowLl  + colR,
                    llRr:   rowLl  + colRr,
                    llRrr:  rowLl  + colRrr,
                    lllFff: rowLll + colFff,
                    lllFf:  rowLll + colFf,
                    lllF:   rowLll + colF,
                    lllC:   rowLll + colC,
                    lllR:   rowLll + colR,
                    lllRr:  rowLll + colRr,
                    lllRrr: rowLll + colRrr
                );
        }

    }


    public class Torus3NbrhdIndexer
    {
        public Torus3NbrhdIndexer(
            int uuuFff, int uuuFf, int uuuF, int uuuC, int uuuR, int uuuRr, int uuuRrr, 
            int uuFff,  int uuFf,  int uuF,  int uuC,  int uuR,  int uuRr,  int uuRrr, 
            int uFff,   int uFf,   int uf,   int uc,   int ur,   int uRr,   int uRrr, 
            int cFff,   int cFf,   int cf,   int cc,   int cr,   int cRr,   int cRrr, 
            int lFff,   int lFf,   int lf,   int lc,   int lr,   int lRr,   int lRrr, 
            int llFff,  int llFf,  int llF,  int llC,  int llR,  int llRr,  int llRrr, 
            int lllFff, int lllFf, int lllF, int lllC, int lllR, int lllRr, int lllRrr)
        {
            _uuuFff = uuuFff;
            _uuuFf = uuuFf;
            _uuuF = uuuF;
            _uuuC = uuuC;
            _uuuR = uuuR;
            _uuuRr = uuuRr;
            _uuuRrr = uuuRrr;
            _uuFff = uuFff;
            _uuFf = uuFf;
            _uuF = uuF;
            _uuC = uuC;
            _uuR = uuR;
            _uuRr = uuRr;
            _uuRrr = uuRrr;
            _uFff = uFff;
            _uFf = uFf;
            _uf = uf;
            _uc = uc;
            _ur = ur;
            _uRr = uRr;
            _uRrr = uRrr;
            _cFff = cFff;
            _cFf = cFf;
            _cf = cf;
            _cc = cc;
            _cr = cr;
            _cRr = cRr;
            _cRrr = cRrr;
            _lFff = lFff;
            _lFf = lFf;
            _lf = lf;
            _lc = lc;
            _lr = lr;
            _lRr = lRr;
            _lRrr = lRrr;
            _llFff = llFff;
            _llFf = llFf;
            _llF = llF;
            _llC = llC;
            _llR = llR;
            _llRr = llRr;
            _llRrr = llRrr;
            _lllFff = lllFff;
            _lllFf = lllFf;
            _lllF = lllF;
            _lllC = lllC;
            _lllR = lllR;
            _lllRr = lllRr;
            _lllRrr = lllRrr;
        }


        private readonly int _uuuFff;
        private readonly int _uuuFf;
        private readonly int _uuuF;
        private readonly int _uuuC;
        private readonly int _uuuR;
        private readonly int _uuuRr;
        private readonly int _uuuRrr;


        private readonly int _uuFff;
        private readonly int _uuFf;
        private readonly int _uuF;
        private readonly int _uuC;
        private readonly int _uuR;
        private readonly int _uuRr;
        private readonly int _uuRrr;


        private readonly int _uFff;
        private readonly int _uFf;
        private readonly int _uf;
        private readonly int _uc;
        private readonly int _ur;
        private readonly int _uRr;
        private readonly int _uRrr;


        private readonly int _cFff;
        private readonly int _cFf;
        private readonly int _cf;
        private readonly int _cc;
        private readonly int _cr;
        private readonly int _cRr;
        private readonly int _cRrr;


        private readonly int _lFff;
        private readonly int _lFf;
        private readonly int _lf;
        private readonly int _lc;
        private readonly int _lr;
        private readonly int _lRr;
        private readonly int _lRrr;


        private readonly int _llFff;
        private readonly int _llFf;
        private readonly int _llF;
        private readonly int _llC;
        private readonly int _llR;
        private readonly int _llRr;
        private readonly int _llRrr;


        private readonly int _lllFff;
        private readonly int _lllFf;
        private readonly int _lllF;
        private readonly int _lllC;
        private readonly int _lllR;
        private readonly int _lllRr;
        private readonly int _lllRrr;


        public int UuuFff
        {
            get { return _uuuFff; }
        }

        public int UuuFf
        {
            get { return _uuuFf; }
        }

        public int UuuF
        {
            get { return _uuuF; }
        }

        public int UuuC
        {
            get { return _uuuC; }
        }

        public int UuuR
        {
            get { return _uuuR; }
        }

        public int UuuRr
        {
            get { return _uuuRr; }
        }

        public int UuuRrr
        {
            get { return _uuuRrr; }
        }


        public int UuFff
        {
            get { return _uuFff; }
        }

        public int UuFf
        {
            get { return _uuFf; }
        }

        public int UuF
        {
            get { return _uuF; }
        }

        public int UuC
        {
            get { return _uuC; }
        }

        public int UuR
        {
            get { return _uuR; }
        }

        public int UuRr
        {
            get { return _uuRr; }
        }

        public int UuRrr
        {
            get { return _uuRrr; }
        }


        public int UFff
        {
            get { return _uFff; }
        }

        public int UFf
        {
            get { return _uFf; }
        }

        public int UF
        {
            get { return _uf; }
        }

        public int UC
        {
            get { return _uc; }
        }

        public int UR
        {
            get { return _ur; }
        }

        public int URr
        {
            get { return _uRr; }
        }

        public int URrr
        {
            get { return _uRrr; }
        }


        public int CFff
        {
            get { return _cFff; }
        }

        public int CFf
        {
            get { return _cFf; }
        }

        public int CF
        {
            get { return _cf; }
        }

        public int CC
        {
            get { return _cc; }
        }

        public int CR
        {
            get { return _cr; }
        }

        public int CRr
        {
            get { return _cRr; }
        }

        public int CRrr
        {
            get { return _cRrr; }
        }


        public int LFff
        {
            get { return _lFff; }
        }

        public int LFf
        {
            get { return _lFf; }
        }

        public int LF
        {
            get { return _lf; }
        }

        public int LC
        {
            get { return _lc; }
        }

        public int LR
        {
            get { return _lr; }
        }

        public int LRr
        {
            get { return _lRr; }
        }

        public int LRrr
        {
            get { return _lRrr; }
        }


        public int LlFff
        {
            get { return _llFff; }
        }

        public int LlFf
        {
            get { return _llFf; }
        }

        public int LlF
        {
            get { return _llF; }
        }

        public int LlC
        {
            get { return _llC; }
        }

        public int LlR
        {
            get { return _llR; }
        }

        public int LlRr
        {
            get { return _llRr; }
        }

        public int LlRrr
        {
            get { return _llRrr; }
        }


        public int LllFff
        {
            get { return _lllFff; }
        }

        public int LllFf
        {
            get { return _lllFf; }
        }

        public int LllF
        {
            get { return _lllF; }
        }

        public int LllC
        {
            get { return _lllC; }
        }

        public int LllR
        {
            get { return _lllR; }
        }

        public int LllRr
        {
            get { return _lllRr; }
        }

        public int LllRrr
        {
            get { return _lllRrr; }
        }
    }
}
