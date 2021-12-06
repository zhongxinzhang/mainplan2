using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.SS.Util;
using System.Text.RegularExpressions;

namespace Mailplan2
{
    class Program
    {
        static bool sendtag = false;
        static void Main(string[] args)
        {
            Console.WriteLine("发送邮件！！");
            ExportExcelStorage();
            //if (sendtag == true)
            //{
                SendEmail();
            //}
        }

        //Material Arrival Report
        static string filnam = ConfigurationManager.AppSettings["FileTitleName"].ToString();
        static string a = DateTime.Now.ToString("yyyyMMdd");//这个路径 有 ：等符号，路径不支持
        static string pathUnchange = @"d:\excel\" + filnam + a; //导出的Excel 要存放的路径
        static string ExportExcelStoragePath = "";//货架上商品统计报表路径

        /// <summary>
        /// Material Arrival Report
        /// </summary>
        /// <returns></returns>
        public static string ExportExcelStorage()
        {
            //数据查询
            SqlConnection conn = null;
            DataTable dt = null;
            SqlCommand comm = null;
            try
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["SQLServer"].ToString();
                conn = new SqlConnection(connectionstring);
                conn.Open();
                comm = conn.CreateCommand();
                comm.Connection = conn;

                //var sql = "select d.ttdte as a1,d.tprod as a2,idesc as a3,iumr as a4,d.tqty as a5,'N' as a6,d.tref as a7,thlin as a8,tvend as a9 ,rtrim(ltrim(vndnam)) as a10 ,'' as a11 ,'' as a12 from (";
                //sql = sql + " select tprod, thlin, tref, tvend, sum(tqty) as tqty, min(ttdte) as ttdte from(";
                //sql = sql + " select tprod, thlin, tref, ttype, tvend,case ltrim(rtrim(ttype)) when 'P' then - sum(tqty) else sum(tqty) end as tqty, min(ttdte) as ttdte";
                //sql = sql + " from(select tprod, thlin, tref, ttype, tvend, tqty, ttdte from ith where rtrim(ttype) + tloct in ('NRINSP', 'PRS', 'NRS', 'NAR', 'N0012', 'P1CBA01', 'P0012', 'PC012', 'PAR')) a";
                //sql = sql + " group by tprod, thlin, tref, ttype, tvend) c group by tprod, thlin, tref, tvend having sum(tqty) > 0";
                //sql = sql + " ) d left join iim on d.tprod = iprod and iid = 'IM' left join avm on tvend = vendor where d.ttdte > 20190101 order by d.tref";
                //comm.CommandText = sql;
                //comm.ExecuteNonQuery();
                comm.CommandText = "apc_data_pri";
                comm.CommandType = CommandType.StoredProcedure;
                //comm.Parameters.AddWithValue("@instring1", "990");
                //comm.Parameters.AddWithValue("@instring2", "");
                //comm.Parameters.AddWithValue("@instring3", "");
                //comm.Parameters.AddWithValue("@instring4", "");
                //comm.Parameters.AddWithValue("@instring5", "");
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(comm);
                dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                if (dt.Rows.Count > 1) { sendtag = true; }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }

            //创建一个excel工作簿
            #region 创建excel
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet     //创建一个页
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //创建excel 单元格式1
            //ICellStyle cellStyle = book.CreateCellStyle();
            //cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            //cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            ////水平对齐
            //cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            ////垂直对齐
            //cellStyle.VerticalAlignment = VerticalAlignment.Center;
            ////设置字体
            //IFont font = book.CreateFont();
            //font.FontHeightInPoints = 15;
            //font.FontName = "微软雅黑";
            //font.Boldweight = (short)FontBoldWeight.Bold;
            //cellStyle.SetFont(font);

            //设置单元格 的宽高
            //sheet1.DefaultColumnWidth = 1 * 25;  //宽度
            //sheet1.DefaultRowHeightInPoints = 25;  //高度
            sheet1.SetColumnWidth(0, 14 * 256);
            sheet1.SetColumnWidth(1, 20 * 256);
            sheet1.SetColumnWidth(2, 30 * 256);
            sheet1.SetColumnWidth(3, 13 * 256);
            sheet1.SetColumnWidth(4, 13 * 256);
            sheet1.SetColumnWidth(5, 13 * 256);
            sheet1.SetColumnWidth(8, 20 * 256);
            sheet1.SetColumnWidth(9, 30 * 256);
            sheet1.SetColumnWidth(10, 20 * 256);
            //创建一行
            IRow row = sheet1.CreateRow(0);
            //创建一列
            ICell cell = row.CreateCell(0);
            ICellStyle cellStyle = book.CreateCellStyle();////创建样式对象
            ICellStyle cellStyle1 = book.CreateCellStyle();////创建样式对象
            IFont font = book.CreateFont(); //创建一个字体样式对象
            font.FontName = "方正舒体"; //和excel里面的字体对应
            font.FontHeightInPoints = 16;//字体大小
            font.IsBold = true;//字体加粗
            cellStyle.SetFont(font); //将字体样式赋给样式对象
            //设置单元格的样式：水平对齐居中
            cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//垂直对齐
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平对齐  这两个在这里不起作用
            cell.CellStyle = cellStyle;
            //把样式赋给单元格   
            //设置背景颜色
            //cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
            //cellStyle.FillPattern = FillPattern.SolidForeground;
            //cellStyle.SetFont(font1);
            //给sheet1添加第一行的头部标题           
            #endregion

            IFont font0 = book.CreateFont();
            font0.FontHeightInPoints = 18;
            font0.FontName = "Arial";
            font0.IsBold = true;
            cellStyle1.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cellStyle1.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cellStyle1.SetFont(font0);

            ICell rowICell0 = row.CreateCell(0);
            ICell rowICell1 = row.CreateCell(1);
            ICell rowICell2 = row.CreateCell(2);
            ICell rowICell3 = row.CreateCell(3);
            ICell rowICell4 = row.CreateCell(4);
            ICell rowICell5 = row.CreateCell(5);
            ICell rowICell6 = row.CreateCell(6);
            ICell rowICell7 = row.CreateCell(7);
            ICell rowICell8 = row.CreateCell(8);
            ICell rowICell9 = row.CreateCell(9);
            ICell rowICell10 = row.CreateCell(10);
            //给第一单元格添加内容
            var SubjectKey = ConfigurationManager.AppSettings["SubjectKey"].ToString();
            rowICell0.SetCellValue(SubjectKey);
            rowICell0.CellStyle = cellStyle1;
            //合并单元格
            sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));

            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(1);
            row1.CreateCell(0).SetCellValue("交检日期/Trans Date");
            row1.CreateCell(1).SetCellValue("零件号码/Item No.");
            row1.CreateCell(2).SetCellValue("物料描述/Desc.");
            row1.CreateCell(3).SetCellValue("单位/Stock UM");
            row1.CreateCell(4).SetCellValue("交检数量/Trans Qty");
            row1.CreateCell(5).SetCellValue("Trans Type");
            row1.CreateCell(6).SetCellValue("Ref No    ");
            row1.CreateCell(7).SetCellValue("行号/Line");
            row1.CreateCell(8).SetCellValue("供应商号/Vendor Code");
            row1.CreateCell(9).SetCellValue("供应商名称/Vendor Name");
            row1.CreateCell(10).SetCellValue("状态");
            //将数据逐步写入sheet1各个行
            int i = 1;

            foreach (DataRow dr in dt.Rows)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                //创建单元格并设置它的值 ID
                rowtemp.CreateCell(0).SetCellValue(dr["a1"].ToString());
                rowtemp.CreateCell(1).SetCellValue(dr["a2"].ToString());
                rowtemp.CreateCell(2).SetCellValue(dr["a3"].ToString());
                rowtemp.CreateCell(3).SetCellValue(dr["a4"].ToString());
                double doubV = 0;
                double.TryParse(dr["a5"].ToString(), out doubV);
                rowtemp.CreateCell(4).SetCellValue(doubV);
                rowtemp.CreateCell(5).SetCellValue(dr["a6"].ToString());
                rowtemp.CreateCell(6).SetCellValue(dr["a7"].ToString());
                int intV = 0;
                int.TryParse(dr["a8"].ToString(), out intV);
                rowtemp.CreateCell(7).SetCellValue(intV);
                rowtemp.CreateCell(8).SetCellValue(dr["a9"].ToString());
                rowtemp.CreateCell(9).SetCellValue(dr["a10"].ToString());
                rowtemp.CreateCell(10).SetCellValue(dr["a11"].ToString());
                ++i;
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string completePath = pathUnchange + ".xls";

            using (FileStream fs = new FileStream(completePath, FileMode.Create, FileAccess.Write))
            {
                byte[] datab = ms.ToArray();
                fs.Write(datab, 0, datab.Length);
                fs.Flush();
                fs.Dispose();
            }
            ms.Close();
            ms.Dispose();
            ExportExcelStoragePath = completePath;
            return ExportExcelStoragePath;
        }
        ///// <summary>
        /////发送邮件  导入命名空间 using System.Net.Mail;  
        ///// </summary>
        public static void SendEmail()
        {
            //----在这里用configuration 那个类 和读取连接字符串似得 读取刚才的key ---
            //配置文件的方式读取
            var FromKey = ConfigurationManager.AppSettings["FromKey"].ToString();
            var ToAddKey = ConfigurationManager.AppSettings["ToAddKey"].ToString();
            var SubjectKey = ConfigurationManager.AppSettings["SubjectKey"].ToString();
            var BodyKey = ConfigurationManager.AppSettings["BodyKey"].ToString();
            var CCAddKey = ConfigurationManager.AppSettings["CCAddKey"].ToString();
            var EmailKey = ConfigurationManager.AppSettings["EmailKey"].ToString();
            var PasswordKey = ConfigurationManager.AppSettings["PasswordKey"].ToString();

            string bodycontent = null;
            //string path1 = System.IO.Directory.GetCurrentDirectory();

            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader("d:\\plan\\plan2\\setup.ini"))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        bodycontent = bodycontent + line;
                    }
                }
            }
            catch (Exception e)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            //声明一个Mail对象     
            MailMessage mymail = new MailMessage();
            mymail.Attachments.Add(new Attachment(ExportExcelStoragePath));
            //发件人地址 
            //如是自己，在此输入自己的邮箱   
            mymail.From = new MailAddress(FromKey);
            //收件人地址
            string[] sArray = Regex.Split(ToAddKey, ",", RegexOptions.IgnoreCase);
            foreach (string sendstr in sArray) mymail.To.Add(sendstr);
            //邮件主题
            mymail.Subject = SubjectKey;
            //邮件标题编码
            mymail.SubjectEncoding = System.Text.Encoding.UTF8;
            //发送邮件的内容
            //mymail.Body = BodyKey;           
            mymail.Body = bodycontent;

            //邮件内容编码
            mymail.BodyEncoding = System.Text.Encoding.UTF8;
            //添加附件
            //Attachment myfiles = new Attachment(tb_Attachment.PostedFile.FileName);
            //mymail.Attachments.Add(myfiles);   
            //抄送到其他邮箱
            sArray = Regex.Split(CCAddKey, ",", RegexOptions.IgnoreCase);
            foreach (string sendstr in sArray) mymail.CC.Add(new MailAddress(sendstr));
            //是否是HTML邮件
            mymail.IsBodyHtml = true;
            //邮件优先级
            mymail.Priority = MailPriority.High;
            //创建一个邮件服务器类  
            SmtpClient myclient = new SmtpClient();
            myclient.Host = "smtp.arrowheadproducts.cn";
            //SMTP服务端口s
            myclient.Port = 25;
            myclient.EnableSsl = false;
            //验证登录  
            myclient.Credentials = new NetworkCredential(EmailKey, PasswordKey);//"@"输入有效的邮件名, "*"输入有效的密码
            myclient.Send(mymail);
            Console.WriteLine("mail send success！");
        }
    }
}
