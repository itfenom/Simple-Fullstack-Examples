using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using csmatio.io;
using csmatio.types;
using Playground.Core.AdoNet;
using Playground.Core.Utilities;

// ReSharper disable PossibleNullReferenceException
namespace Playground.ConsoleApp.BawDataViewer
{
    public class OiBinaryOutput
    {
        private readonly string _file;
        private readonly int _productKey;

        public OiBinaryOutput(string product, string file)
        {
            _file = file;
            //_productKey = Convert.ToInt32(DAL.TQTGAAS.ExecuteScalar($@"SELECT SMAP_MAIN_PK FROM PROBEBUILDER.STEPPERMAP_MAIN WHERE PRODUCT='{product}'"));
            _productKey = 947;
        }

        public List<string> ProcessOiOutput()
        {
            var retVal = new List<string>();
            var oiOutput = GetOiOutput();
            if (oiOutput.Count > 0)
            {
                int groupSize = 40; //uncheckedRows per statement; 40 seems to be about optimal based on experimentation
                int groupStart = 0;

                while (groupStart < oiOutput.Count)
                {
                    var sqlStr = new StringBuilder("MERGE INTO PROBEBUILDER.AOI_PROD_SETUP A\nUSING (");
                    sqlStr.AppendLine($"\nSELECT {oiOutput[groupStart].ProductKey} AS SMAP_MAIN_FK,");
                    sqlStr.AppendLine($"{oiOutput[groupStart].MergeOrder} AS MERGE_ORDER, ");
                    sqlStr.AppendLine($"{oiOutput[groupStart].OffSetX} AS OFFSET_X, ");
                    sqlStr.AppendLine($"{oiOutput[groupStart].OffSetY} AS OFFSET_Y, ");
                    sqlStr.AppendLine($"{GetFormattedDirNameExtensionValue(oiOutput[groupStart].DirNameExtension)} AS DIRNAME_EXTENSION, ");
                    sqlStr.AppendLine($"{oiOutput[groupStart].Rotation} AS ROTATION, ");
                    sqlStr.AppendLine("SYSDATE AS CREATION_DATE, ");
                    sqlStr.AppendLine("'KM031369' AS CREATION_USERID, ");
                    sqlStr.AppendLine("SYSDATE AS LAST_MODIFIED_DATE, ");
                    sqlStr.AppendLine("'KM031369' AS LAST_MODIFIED_USERID, ");
                    sqlStr.AppendLine($"{oiOutput[groupStart].IsOptional} AS ISOPTIONAL ");
                    sqlStr.AppendLine(" FROM DUAL");

                    for (int i = groupStart + 1; i < oiOutput.Count && i < groupStart + groupSize; i++)
                    {
                        sqlStr.AppendLine("\nUNION ALL SELECT ");
                        sqlStr.AppendLine($"{oiOutput[i].ProductKey}, ");
                        sqlStr.AppendLine($"{oiOutput[i].MergeOrder}, ");
                        sqlStr.AppendLine($"{oiOutput[i].OffSetX}, ");
                        sqlStr.AppendLine($"{oiOutput[i].OffSetY}, ");
                        sqlStr.AppendLine($"{GetFormattedDirNameExtensionValue(oiOutput[i].DirNameExtension)}, ");
                        sqlStr.AppendLine($"{oiOutput[i].Rotation}, ");
                        sqlStr.AppendLine("SYSDATE, ");
                        sqlStr.AppendLine($"'{"KM031369"}', ");
                        sqlStr.AppendLine("SYSDATE, ");
                        sqlStr.AppendLine($"'{"KM031369"}', ");
                        sqlStr.AppendLine($"{oiOutput[i].IsOptional} ");
                        sqlStr.AppendLine(" FROM DUAL");
                    }

                    sqlStr.AppendLine(
                        "\n) B"
                        + "\nON"
                        + "\n(B.SMAP_MAIN_FK = A.SMAP_MAIN_FK"
                        + "\nAND B.MERGE_ORDER = A.MERGE_ORDER"
                        + "\nAND B.DIRNAME_EXTENSION = A.DIRNAME_EXTENSION)"

                        + "\nWHEN MATCHED THEN"

                        + "\nUPDATE SET A.OFFSET_X = B.OFFSET_X, "
                        + "\nA.OFFSET_Y = B.OFFSET_Y, "
                        + "\nA.ROTATION = B.ROTATION, "
                        + "\nA.ISOPTIONAL = B.ISOPTIONAL "

                        + "\nWHEN NOT MATCHED THEN"

                        + "\nINSERT (SMAP_MAIN_FK, MERGE_ORDER, OFFSET_X, OFFSET_Y, DIRNAME_EXTENSION, ROTATION, CREATION_DATE, CREATION_USERID, LAST_MODIFIED_DATE, LAST_MODIFIED_USERID, ISOPTIONAL)"
                        + "\nVALUES (B.SMAP_MAIN_FK, B.MERGE_ORDER, B.OFFSET_X, B.OFFSET_Y, B.DIRNAME_EXTENSION, B.ROTATION, B.CREATION_DATE, B.CREATION_USERID, B.LAST_MODIFIED_DATE, B.LAST_MODIFIED_USERID, B.ISOPTIONAL)");

                    groupStart += groupSize;
                    retVal.Add(sqlStr.ToString());
                }
            }
            else
            {
                throw new Exception("Could not retrieve any data from OI output!");
            }

            return retVal;
        }

        private string GetFormattedDirNameExtensionValue(string inputVal)
        {
            return string.IsNullOrEmpty(inputVal) ? "NULL" : "'" + HelperTools.FormatSqlString(inputVal) + "'";
        }

        private List<OiObject> GetOiOutput()
        {
            var retVal = new List<OiObject>();
            var mfr = new MatFileReader(_file);
            var mlStruct = mfr.Content["OIout"] as MLStructure;

            /*
             MLChar = string
             MLDouble = double
             MLSingle = float
             */

            // ReSharper disable once PossibleNullReferenceException
            for (int i = 0; i < mlStruct.Size; i++)
            {
                var item = new OiObject { ProductKey = _productKey, MergeOrder = (i + 1) };

                var dirNameExtension = mlStruct["dirname_extension", i] as MLChar;

                item.DirNameExtension = dirNameExtension.GetString(0);

                //OffSetX
                if (mlStruct["offsetx", i].IsDouble)
                {
                    item.OffSetX = (mlStruct["offsetx", i] as MLDouble).Get(0);
                }
                else if (mlStruct["offsetx", i].IsSingle)
                {
                    item.OffSetX = (mlStruct["offsetx", i] as MLSingle).Get(0);
                }

                //OffSetY
                if (mlStruct["offsety", i].IsDouble)
                {
                    item.OffSetY = (mlStruct["offsety", i] as MLDouble).Get(0);
                }
                else if (mlStruct["offsety", i].IsSingle)
                {
                    item.OffSetY = (mlStruct["offsety", i] as MLSingle).Get(0);
                }

                //Rotation or (phi)
                if (mlStruct["phi", i].IsDouble)
                {
                    item.Rotation = (mlStruct["phi", i] as MLDouble).Get(0);
                }
                else if (mlStruct["phi", i].IsSingle)
                {
                    item.Rotation = (mlStruct["phi", i] as MLSingle).Get(0);
                }

                //IsOptional
                if (mlStruct["isOptional", i].IsDouble)
                {
                    item.IsOptional = (mlStruct["isOptional", i] as MLDouble).Get(0);
                }
                else if (mlStruct["isOptional", i].IsSingle)
                {
                    item.IsOptional = (mlStruct["isOptional", i] as MLSingle).Get(0);
                }

                retVal.Add(item);

            }

            return retVal;
        }
    }

    public class OiObject
    {
        public int ProductKey { get; set; }
        public int MergeOrder { get; set; }
        public double OffSetX { get; set; }
        public double OffSetY { get; set; }
        public string DirNameExtension { get; set; }
        public double Rotation { get; set; }
        public double IsOptional { get; set; }
    }
}
