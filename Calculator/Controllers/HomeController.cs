using Calculator_CSharp.Models;
using System.Web.Mvc;
using System;
using System.Web;

namespace Calculator_CSharp.Controllers
{
    public class HomeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(Calculator calculator)
        {
            return View(calculator);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Calculate(Calculator calculator)
        {
            calculator.Result = CalculateResult(calculator);

            return RedirectToAction("Index", calculator);
        }

        public decimal CalculateResult(Calculator calculator)
        {
            var result = 0.0m;

            switch (calculator.Operator)
            {
                case "+":
                    result = calculator.LeftOperand + calculator.RightOperand;
                    break;
                case "-":
                    result = calculator.LeftOperand - calculator.RightOperand;
                    break;
                case "*":
                    result = calculator.LeftOperand * calculator.RightOperand;
                    break;
                case "/":
                    result = calculator.LeftOperand / calculator.RightOperand;
                    break;
            }

            return result;
        }
    }
}