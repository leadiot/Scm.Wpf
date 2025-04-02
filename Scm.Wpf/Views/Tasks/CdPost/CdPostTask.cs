using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using SqlSugar;
using System.IO;

namespace Com.Scm.Wpf.Views.Tasks.CdPost
{
    /// <summary>
    /// 成都邮政扫描数据导出
    /// </summary>
    public class CdPostTask : ATask
    {
        public override string Name
        {
            get { return "成都邮政数据处理"; }
        }

        public override string Help
        {
            get { return ""; }
        }

        public override void Run()
        {
            var file = @"D:\billinfo.xlsx";
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var client = GetClient(DbType.Sqlite, "D:\\DarianToolsManger.db"))
            {
                client.Open();

                var list = client.Queryable<BusinessA>()
                    .OrderBy(a => a.AddTime)
                    .ToList();

                var list2 = new List<BusinessB>();
                BusinessB dbo = new BusinessB();
                foreach (var item in list)
                {
                    if (!isScan(item))
                    {
                        continue;
                    }

                    if (dbo.billno != item.BillNo)
                    {
                        dbo = list2.Find(a => a.billno == item.BillNo);
                        if (dbo == null)
                        {
                            dbo = new BusinessB();
                            dbo.billno = item.BillNo;
                            dbo.epccode = item.EPCCode;
                            list2.Add(dbo);
                        }
                    }

                    if (item.BussinessCode == "00001")
                    {
                        dbo.B00001 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00002")
                    {
                        dbo.B00002 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00003")
                    {
                        dbo.B00003 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00004")
                    {
                        dbo.B00004 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00005")
                    {
                        if (isOut(item))
                        {
                            dbo.C00005 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00005 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00006")
                    {
                        dbo.B00006 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00007")
                    {
                        dbo.B00007 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00008")
                    {
                        dbo.B00008 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00009")
                    {
                        dbo.B00009 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00010")
                    {
                        dbo.B00010 = item.AddTime;
                        continue;
                    }
                    if (item.BussinessCode == "00011")
                    {
                        if (isOut(item))
                        {
                            dbo.C00011 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00011 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00012")
                    {
                        if (isOut(item))
                        {
                            dbo.C00012 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00012 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00013")
                    {
                        if (isOut(item))
                        {
                            dbo.C00013 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00013 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00014")
                    {
                        if (isOut(item))
                        {
                            dbo.C00014 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00014 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00015")
                    {
                        if (isOut(item))
                        {
                            dbo.C00015 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00015 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00016")
                    {
                        if (isOut(item))
                        {
                            dbo.C00016 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00016 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00017")
                    {
                        if (isOut(item))
                        {
                            dbo.C00017 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00017 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00018")
                    {
                        if (isOut(item))
                        {
                            dbo.C00018 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00018 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00019")
                    {
                        if (isOut(item))
                        {
                            dbo.C00019 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00019 = item.AddTime;
                        }
                        continue;
                    }
                    if (item.BussinessCode == "00020")
                    {
                        if (isOut(item))
                        {
                            dbo.C00020 = item.AddTime;
                        }
                        else
                        {
                            dbo.B00020 = item.AddTime;
                        }
                        continue;
                    }
                }

                MiniExcel.SaveAs(file, list2);
            }
        }

        public bool isOut(BusinessA item)
        {
            return item.InOutFlag == "1";
        }

        public bool isScan(BusinessA item)
        {
            return item.ScanStatue == "1";
        }
    }

    [SugarTable("WayBillBussiness")]
    public class BusinessA
    {
        public string BillNo { get; set; }
        public string EPCCode { get; set; }
        public string BussinessCode { get; set; }
        public string BillStatue { get; set; }
        public string DeviceCode { get; set; }
        public string TidCode { get; set; }
        public string OrderCode { get; set; }
        public string AddTime { get; set; }
        public string ScanStatue { get; set; }
        public string UpLoadStatue { get; set; }
        public string InOutFlag { get; set; }
    }

    public class BusinessB
    {
        [ExcelColumnName("BillNo")]
        public string billno { get; set; }
        [ExcelColumnName("EPCCode")]
        public string epccode { get; set; }
        /// <summary>
        /// EPC绑定
        /// </summary>
        [ExcelColumnName("EPC绑定01")]
        public string B00001 { get; set; }
        /// <summary>
        /// EPC绑定
        /// </summary>
        [ExcelColumnName("EPC绑定02")]
        public string B00002 { get; set; }
        /// <summary>
        /// 成都华为仓出垛口采集点
        /// </summary>
        [ExcelColumnName("成都华为仓出垛口采集点")]
        public string B00003 { get; set; }
        /// <summary>
        /// 成都处理中心
        /// </summary>
        [ExcelColumnName("成都处理中心")]
        public string B00004 { get; set; }
        /// <summary>
        /// 成都贵品区（入）
        /// </summary>
        [ExcelColumnName("成都贵品区（入）")]
        public string B00005 { get; set; }
        /// <summary>
        /// 成都贵品区（出）
        /// </summary>
        [ExcelColumnName("成都贵品区（出）")]
        public string C00005 { get; set; }
        /// <summary>
        /// 发往绵阳出垛口采集点
        /// </summary>
        [ExcelColumnName("发往绵阳出垛口采集点")]
        public string B00006 { get; set; }
        /// <summary>
        /// 发往江油出垛口采集点
        /// </summary>
        [ExcelColumnName("发往江油出垛口采集点")]
        public string B00007 { get; set; }
        /// <summary>
        /// 发往三台出垛口采集点
        /// </summary>
        [ExcelColumnName("发往三台出垛口采集点")]
        public string B00008 { get; set; }
        /// <summary>
        /// 绵阳处理中心（入）
        /// </summary>
        [ExcelColumnName("绵阳处理中心（入）")]
        public string B00009 { get; set; }
        /// <summary>
        /// 绵阳处理中心（出）
        /// </summary>
        [ExcelColumnName("绵阳处理中心（出）")]
        public string B00010 { get; set; }
        /// <summary>
        /// 科创园采集点（入）
        /// </summary>
        [ExcelColumnName("科创园采集点（入）")]
        public string B00011 { get; set; }
        /// <summary>
        /// 科创园采集点（出）
        /// </summary>
        [ExcelColumnName("科创园采集点（出）")]
        public string C00011 { get; set; }
        /// <summary>
        /// 西山采集点（入）
        /// </summary>
        [ExcelColumnName("西山采集点（入）")]
        public string B00012 { get; set; }
        /// <summary>
        /// 西山采集点（出）
        /// </summary>
        [ExcelColumnName("西山采集点（出）")]
        public string C00012 { get; set; }
        /// <summary>
        /// 江油采集点（入）
        /// </summary>
        [ExcelColumnName("江油采集点（入）")]
        public string B00013 { get; set; }
        /// <summary>
        /// 江油采集点（出）
        /// </summary>
        [ExcelColumnName("江油采集点（出）")]
        public string C00013 { get; set; }
        /// <summary>
        /// 东津采集点（入）
        /// </summary>
        [ExcelColumnName("东津采集点（入）")]
        public string B00014 { get; set; }
        /// <summary>
        /// 东津采集点（出）
        /// </summary>
        [ExcelColumnName("东津采集点（出）")]
        public string C00014 { get; set; }
        /// <summary>
        /// 高新区采集点（入）
        /// </summary>
        [ExcelColumnName("高新区采集点（入）")]
        public string B00015 { get; set; }
        /// <summary>
        /// 高新区采集点（出）
        /// </summary>
        [ExcelColumnName("高新区采集点（出）")]
        public string C00015 { get; set; }
        /// <summary>
        /// 经开区采集点（入）
        /// </summary>
        [ExcelColumnName("经开区采集点（入）")]
        public string B00016 { get; set; }
        /// <summary>
        /// 经开区采集点（出）
        /// </summary>
        [ExcelColumnName("经开区采集点（出）")]
        public string C00016 { get; set; }
        /// <summary>
        /// 三台采集点（入）
        /// </summary>
        [ExcelColumnName("三台采集点（入）")]
        public string B00017 { get; set; }
        /// <summary>
        /// 三台采集点（出）
        /// </summary>
        [ExcelColumnName("三台采集点（出）")]
        public string C00017 { get; set; }
        /// <summary>
        /// 永兴采集点（入）
        /// </summary>
        [ExcelColumnName("永兴采集点（入）")]
        public string B00018 { get; set; }
        /// <summary>
        /// 永兴采集点（出）
        /// </summary>
        [ExcelColumnName("永兴采集点（出）")]
        public string C00018 { get; set; }
        /// <summary>
        /// 文胜采集点（入）
        /// </summary>
        [ExcelColumnName("文胜采集点（入）")]
        public string B00019 { get; set; }
        /// <summary>
        /// 文胜采集点（出）
        /// </summary>
        [ExcelColumnName("文胜采集点（出）")]
        public string C00019 { get; set; }
        /// <summary>
        /// 西科大采集点（入）
        /// </summary>
        [ExcelColumnName("西科大采集点（入）")]
        public string B00020 { get; set; }
        /// <summary>
        /// 西科大采集点（出）
        /// </summary>
        [ExcelColumnName("西科大采集点（出）")]
        public string C00020 { get; set; }
    }
}
