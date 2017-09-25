
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using okcopy.Common;
using okcopy.DataAccess;

namespace okcopy.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Query()
        {
            using (var db = new SqlHelper())
            {
                var res = db.Query(" select * from OpenOrder where PlatID=@PlatID", new { PlatID = 3 });

                return Json(res);
            }
        }
        [HttpPost]
        public ActionResult GetUserInformation(int platid,string mtaccount)
        {
            using (var db = new SqlHelper())
            {
                #region 收益率
                var res = db.Query(@"select rate   from
                ((select sum((Profit/Balance)*100+1) as rate   from OpenOrder  where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount)
                union
                (select sum((Profit/Balance)*100+1) as rate  from CloseOrder where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount))", new { PlatID = platid, MTAccount = mtaccount });

                decimal allrate = 1;//收益率
                foreach (var i in res)
                {
                    if (null != i)
                    {
                        var rate = Decimal.Parse(i["rate"].ToString());
                        allrate = rate * allrate;
                    }
                }
                #endregion

                #region 绝对收益率
                decimal absrate = 0;//收益率
                var ratebalance = db.Query(" select top1 Balance from MTAccount where PlatID=@PlatID and MTAccount=@MTAccount", new { PlatID = platid, MTAccount = mtaccount });

                var rateprofit = db.Query(@"select top1 Profit from
((select sum(Profit)   from
((select sum(Profit) as Profit from OpenOrder  where OrderType=6 and  Profit>0 and PlatID=@PlatID and MTAccount=@MTAccount)
union
(select sum(Profit) as Profit from CloseOrder where OrderType=6 and Profit>0 and PlatID=@PlatID and MTAccount=@MTAccount )))
-
(select sum(Profit)  from
((select sum(Profit) as Profit from OpenOrder  where OrderType=6 and  Profit<0 and PlatID=@PlatID and MTAccount=@MTAccount)
union
(select sum(Profit) as Profit from CloseOrder where OrderType=6 and Profit<0 and PlatID=@PlatID and MTAccount=@MTAccount )))  as Profit )", new { PlatID = platid, MTAccount = mtaccount });

                if (null != ratebalance && null != rateprofit)
                {
                    var absBalance = Decimal.Parse(ratebalance[0]["Balance"].ToString());
                    var absProfit = Decimal.Parse(rateprofit[0]["Profit"].ToString());
                    absrate = (absBalance - absProfit) / absProfit * 100;
                }

                #endregion

                #region 日均收益率
                var startdate = db.Query(@"select top1 Date from
                        ((select top1 OpenDate as Date from OpenOrder  where OrderType in(0,1,6,7) and                         
                        PlatID=@PlatID and MTAccount=@MTAccount order by  OpenOrder   )
                        union
                        (select top1 CloseDate as Date from CloseOrder where OrderType in(0,1,6,7) and                         
                        PlatID=@PlatID and MTAccount=@MTAccount order by  CloseDate ))
                        order by Date ", new { PlatID = platid, MTAccount = mtaccount });
                var enddate = db.Query(@" select top1 Date from
                        ((select top1 OpenDate as Date from OpenOrder  where OrderType in(0,1,6,7) and                         
                        PlatID=@PlatID and MTAccount=@MTAccount order by  OpenOrder  desc )
                        union
                        (select top1 CloseDate as Date from CloseOrder where OrderType in(0,1,6,7) and                         
                        PlatID=@PlatID and MTAccount=@MTAccount order by  CloseDate  desc))
                        order by Date desc", new { PlatID = platid, MTAccount = mtaccount });

                var profitres = db.Query(@"select Profit   from
                ((select sum(Profit) as Profit   from OpenOrder  where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount)
                union
                (select sum(Profit) as Profit  from CloseOrder where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount))", new { PlatID = platid, MTAccount = mtaccount });
                decimal allprofit = 0;//总盈利
                int daydvalue = 0;//总交易天数
                int monthdvalue = 0;//总交易月数
                decimal dayrate = 0;//日均收益率
                decimal monthrate = 0;//月均收益率
                if (null != startdate && null != enddate && null != profitres)
                {
                    var starttime = DateTime.Parse(startdate[0]["Date"].ToString());
                    var endtime = DateTime.Parse(enddate[0]["Date"].ToString());
                    daydvalue = endtime.Subtract(starttime).Days;
                    monthdvalue = endtime.Year * 12 + endtime.Month - starttime.Year * 12 - starttime.Month;
                    allprofit = Decimal.Parse(profitres[0]["Profit"].ToString());
                    dayrate = allprofit / daydvalue*100;
                    monthrate = allprofit / monthdvalue * 100;
                }
                #endregion

                #region 最大回撤率
                //亏损最大的一笔
                var losssql = db.Query(@"select Profit,Equity,Balance,CloseDate  from CloseOrder where OrderType in(0,1,6,7)  and Profit<0 and PlatID=@PlatID and MTAccount=@MTAccount order by Profit", new { PlatID = platid, MTAccount = mtaccount });
                decimal lossrate = 0;//最大回撤率
                if (null != losssql )
                {
                    decimal lossba = Decimal.Parse(losssql[0]["Balance"].ToString());
                    decimal lossprofit=Decimal.Parse(losssql[0]["Profit"].ToString());
                    if (decimal.Parse(losssql[0]["Balance"].ToString()) <= 0)
                    {
                        var lossbalancesql = db.Query(@"select sum(OProfit+CProfit) from
                        ((select sum(Profit) as OProfit,0 as CProfit   from OpenOrder  where OrderType in(0,1,6,7) and PlatID=@PlatID and             MTAccount=@MTAccount  and OpenDate<=" + losssql[0]["CloseDate"] + @" )
                        union
                        (select 0 as OProfit, sum(Profit)  as CProfit from CloseOrder where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount  and CloseDate<=" + losssql[0]["CloseDate"] + @"))
                        ", new { PlatID = platid, MTAccount = mtaccount });
                    }
                    else
                    {
                        lossrate = lossprofit / lossba * 100;
                    }
                }

                #endregion


                #region 余额、净值
                var accsql = db.Query(" select Equity,Balance from MTAccount where PlatID=@PlatID and MTAccount=@MTAccount", new { PlatID = platid, MTAccount = mtaccount });
                decimal accequity = 0;//净值
                decimal accbalance = 0;//余额
                if (null != accsql)
                {
                    accequity = Decimal.Parse(accsql[0]["Equity"].ToString());
                    accbalance = Decimal.Parse(accsql[0]["Balance"].ToString());
                }
                #endregion

                #region 利润、利息
                var cordersql = db.Query(@"select sum(Profit) as Profit,sum(StockFee) as interest  from CloseOrder where OrderType in(0,1,6,7) and PlatID=@PlatID and MTAccount=@MTAccount ", new { PlatID = platid, MTAccount = mtaccount });
                decimal corderprofit = 0;//利润
                decimal corderinterest = 0;//利息
                if (null != accsql)
                {
                    corderprofit = Decimal.Parse(cordersql[0]["Profit"].ToString());
                    corderinterest = Decimal.Parse(cordersql[0]["interest"].ToString());
                }
                #endregion

                #region 出金
                var outprofitsql = db.Query(@"select sum(Profit) as Profit  from CloseOrder where OrderType =6 and Profit<0 and PlatID=@PlatID and MTAccount=@MTAccount ", new { PlatID = platid, MTAccount = mtaccount });
                decimal outprofit = 0;//出金
                if (null != accsql)
                {
                    outprofit = Decimal.Parse(outprofitsql[0]["Profit"].ToString());
                }
                #endregion

                #region 入金
                var joinprofitsql = db.Query(@"select sum(Profit) as Profit  from CloseOrder where OrderType =6 and Profit>0 and PlatID=@PlatID and MTAccount=@MTAccount ", new { PlatID = platid, MTAccount = mtaccount });
                decimal joinprofit = 0;//入金
                if (null != accsql)
                {
                    joinprofit = Decimal.Parse(joinprofitsql[0]["Profit"].ToString());
                }
                #endregion

                return Json(res);
            }
        }
    }
}
