using MathNet.Numerics.Interpolation;
using ResearchWork3.Input;
using System;
using System.Collections.Generic;

namespace ResearchWork3.Calculation_CO
{
    internal class CalculationCollisionCoefficientsOfCo
    {
        //----------------------Calculation collision coefficients of CO with temperature----------------------

        public readonly CubicSpline[][] CoCoeffminiH;
        public readonly CubicSpline[][] CoCoeffminiH2Ortho;
        public readonly CubicSpline[][] CoCoeffminiH2Para;
        public readonly CubicSpline[][] CoCoeffminiHe;

        public CalculationCollisionCoefficientsOfCo()
        {
            var tabCoCoeff = new CalculationCoLevels();

            CoCoeffminiH = new CubicSpline[InputCommonParameters.MaxCoLevel + 1][];
            CoCoeffminiH2Ortho = new CubicSpline[InputCommonParameters.MaxCoLevel + 1][];
            CoCoeffminiH2Para = new CubicSpline[InputCommonParameters.MaxCoLevel + 1][];
            CoCoeffminiHe = new CubicSpline[InputCommonParameters.MaxCoLevel + 1][];

            for (var i = 0; i <= InputCommonParameters.MaxCoLevel; i++)
            {
                CoCoeffminiH[i] = new CubicSpline[InputCommonParameters.MaxCoLevel + 1];
                CoCoeffminiH2Ortho[i] = new CubicSpline[InputCommonParameters.MaxCoLevel + 1];
                CoCoeffminiH2Para[i] = new CubicSpline[InputCommonParameters.MaxCoLevel + 1];
                CoCoeffminiHe[i] = new CubicSpline[InputCommonParameters.MaxCoLevel + 1];

                for (var j = 0; j <= InputCommonParameters.MaxCoLevel; j++)
                {
                    CoCoeffminiH[i][j] = CoCoeff(i, j, tabCoCoeff.TabCoCoeff, InputTablesCo.GetH1Table2015(),
                        InputTablesCo.GetEnergyTable());

                    CoCoeffminiH2Ortho[i][j] = CoCoeff(i, j, tabCoCoeff.TabCoCoeff, InputTablesCo.GetH2TableOrtho2010(),
                        InputTablesCo.GetEnergyTable());

                    CoCoeffminiH2Para[i][j] = CoCoeff(i, j, tabCoCoeff.TabCoCoeff, InputTablesCo.GetH2TablePara2010(),
                        InputTablesCo.GetEnergyTable());

                    CoCoeffminiHe[i][j] = CoCoeff(i, j, tabCoCoeff.TabCoCoeff, InputTablesCo.GetHeTable2002(),
                        InputTablesCo.GetEnergyTable());
                }
            }
        }

        private static CubicSpline CoCoeff(int ii, int jj, double[][] tabCoCoeff, double[][] coTable,
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
                        InputCommonParameters.Hbar *
                        InputCommonParameters.C *
                        (entab[jj] - entab[ii]) /
                        (InputCommonParameters.Kb * coTableX[el]));

                    coTableY[el] = coTable[el][pos + 1] *
                                   (InputCommonParameters.G(jj) / InputCommonParameters.G(ii)) /
                                   tempExp;
                }

                var cohTableSpline = CubicSpline.InterpolateNaturalSorted(coTableX, coTableY);

                return cohTableSpline;
            }

            return null;
        }
    }
}