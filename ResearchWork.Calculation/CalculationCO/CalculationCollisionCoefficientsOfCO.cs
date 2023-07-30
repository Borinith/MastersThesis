using MathNet.Numerics.Interpolation;
using ResearchWork.IO.Input;
using System;
using System.Collections.Generic;

namespace ResearchWork.Calculation.CalculationCO
{
    internal class CalculationCollisionCoefficientsOfCo
    {
        private static readonly Lazy<CalculationCollisionCoefficientsOfCo> Lazy = new(() => new CalculationCollisionCoefficientsOfCo());

        private CubicSpline?[][] _coCoeffMiniH = null!;
        private CubicSpline?[][] _coCoeffMiniH2Ortho = null!;
        private CubicSpline?[][] _coCoeffMiniH2Para = null!;
        private CubicSpline?[][] _coCoeffMiniHe = null!;

        public CalculationCollisionCoefficientsOfCo()
        {
            Init();
        }

        public static CalculationCollisionCoefficientsOfCo Instance => Lazy.Value;

        private void Init()
        {
            _coCoeffMiniH = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1][];
            _coCoeffMiniH2Ortho = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1][];
            _coCoeffMiniH2Para = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1][];
            _coCoeffMiniHe = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1][];

            for (var i = 0; i <= InputCommonParameters.MAX_CO_LEVEL; i++)
            {
                _coCoeffMiniH[i] = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1];
                _coCoeffMiniH2Ortho[i] = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1];
                _coCoeffMiniH2Para[i] = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1];
                _coCoeffMiniHe[i] = new CubicSpline[InputCommonParameters.MAX_CO_LEVEL + 1];

                for (var j = 0; j <= InputCommonParameters.MAX_CO_LEVEL; j++)
                {
                    _coCoeffMiniH[i][j] = CoCoeff(i, j, CalculationCoLevels.Instance.GetTabCoCoeff(), InputTablesCo.Instance.GetH1Table2015(),
                        InputTablesCo.Instance.GetEnergyTable());

                    _coCoeffMiniH2Ortho[i][j] = CoCoeff(i, j, CalculationCoLevels.Instance.GetTabCoCoeff(), InputTablesCo.Instance.GetH2TableOrtho2010(),
                        InputTablesCo.Instance.GetEnergyTable());

                    _coCoeffMiniH2Para[i][j] = CoCoeff(i, j, CalculationCoLevels.Instance.GetTabCoCoeff(), InputTablesCo.Instance.GetH2TablePara2010(),
                        InputTablesCo.Instance.GetEnergyTable());

                    _coCoeffMiniHe[i][j] = CoCoeff(i, j, CalculationCoLevels.Instance.GetTabCoCoeff(), InputTablesCo.Instance.GetHeTable2002(),
                        InputTablesCo.Instance.GetEnergyTable());
                }
            }
        }

        private static CubicSpline? CoCoeff(int ii, int jj, double[][] tabCoCoeff, double[][] coTable,
            IReadOnlyList<double> entab)
        {
            if (ii > jj)
            {
                var pos = 0;

                for (var k = 0; k < tabCoCoeff.GetUpperBound(0) + 1; k++)
                {
                    if ((int)Math.Round(tabCoCoeff[k][0]) == ii && (int)Math.Round(tabCoCoeff[k][1]) == jj)
                    {
                        pos = k;

                        break;
                    }
                }

                var coTableX = new double[coTable.GetUpperBound(0) + 1];
                var coTableY = new double[coTable.GetUpperBound(0) + 1];

                for (var el = 0; el < coTable.GetUpperBound(0) + 1; el++)
                {
                    coTableX[el] = coTable[el][0];
                    coTableY[el] = coTable[el][pos + 1];
                }

                var cohTableSpline = CubicSpline.InterpolateNaturalSorted(coTableX, coTableY);

                return cohTableSpline;
            }

            if (ii < jj)
            {
                var pos = 0;

                for (var k = 0; k < tabCoCoeff.GetUpperBound(0) + 1; k++)
                {
                    if ((int)Math.Round(tabCoCoeff[k][0]) == jj && (int)Math.Round(tabCoCoeff[k][1]) == ii)
                    {
                        pos = k;

                        break;
                    }
                }

                var coTableX = new double[coTable.GetUpperBound(0) + 1];
                var coTableY = new double[coTable.GetUpperBound(0) + 1];

                for (var el = 0; el < coTable.GetUpperBound(0) + 1; el++)
                {
                    coTableX[el] = coTable[el][0];

                    var tempExp = Math.Pow(Math.E,
                        2 *
                        Math.PI *
                        InputCommonParameters.HBAR *
                        InputCommonParameters.C *
                        (entab[jj] - entab[ii]) /
                        (InputCommonParameters.KB * coTableX[el]));

                    coTableY[el] = coTable[el][pos + 1] *
                                   (InputCommonParameters.G(jj) / InputCommonParameters.G(ii)) /
                                   tempExp;
                }

                var cohTableSpline = CubicSpline.InterpolateNaturalSorted(coTableX, coTableY);

                return cohTableSpline;
            }

            return null;
        }

        public CubicSpline?[][] GetCoCoeffMiniH()
        {
            return _coCoeffMiniH;
        }

        public CubicSpline?[][] GetCoCoeffMiniH2Ortho()
        {
            return _coCoeffMiniH2Ortho;
        }

        public CubicSpline?[][] GetCoCoeffMiniH2Para()
        {
            return _coCoeffMiniH2Para;
        }

        public CubicSpline?[][] GetCoCoeffMiniHe()
        {
            return _coCoeffMiniHe;
        }
    }
}