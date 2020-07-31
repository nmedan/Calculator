using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
namespace Calculator
{
    public partial class Form1 : Form
    {
        enum States { NumberEntry, OperationAssigned, ResultCalculated }
        States state;
        enum Errors { UndefinedResult, DivideByZero, InvalidInput, None }
        Errors error;

        enum Operations { Plus, Minus, Multiply, Divide, None }
        Operations operation;
        double firstNumber, secondNumber;
        public Form1()
        {
            InitializeComponent();
            beginningState();

        }

        private void btnDigit_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                Button button = sender as Button;

                if (state != States.OperationAssigned && state != (States.OperationAssigned | States.ResultCalculated))
                {
                    if (state == States.ResultCalculated || (firstNumber == 0 && txtDisplay.Text.Last() != '.' && !(txtDisplay.Text.Contains('.') && txtDisplay.Text.Last() == '0')))
                    {
                        txtDisplay.Text = "";
                        if (state == States.ResultCalculated)
                        {
                            state = States.NumberEntry;
                        }
                        txtDisplay.Text += button.Text;
                    }
                    else
                    {
                        if (txtDisplay.Text.Last() != '.' && !(txtDisplay.Text.Contains('.') && txtDisplay.Text.Last() == '0')) 
                        {
                            txtDisplay.Text = formatResult(firstNumber.ToString()) + button.Text;
                        }
                        else
                        {
                            txtDisplay.Text += button.Text;
                        }
                    }
                    firstNumber = parseNumber(txtDisplay.Text);
                }
                else
                {
                    if (state == (States.ResultCalculated | States.OperationAssigned) || (secondNumber == 0 && txtDisplay.Text.Last() != '.' && !(txtDisplay.Text.Contains('.') && txtDisplay.Text.Last() == '0')))
                    {
                        state = States.OperationAssigned;
                        txtDisplay.Text = "";
                        txtDisplay.Text += button.Text;
                    }
                    else
                    {
                        if (txtDisplay.Text.Last() != '.' && !(txtDisplay.Text.Contains('.') && txtDisplay.Text.Last() == '0'))
                        {
                            txtDisplay.Text = formatResult(secondNumber.ToString()) + button.Text;
                        }
                        else
                        {
                            txtDisplay.Text += button.Text;
                        }
                    }
                    secondNumber = parseNumber(txtDisplay.Text);
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnOperation_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                Button button = sender as Button;
                if (state == States.OperationAssigned)
                {
                    switch (operation)
                    {
                        case Operations.Plus:
                            firstNumber += secondNumber;
                            break;
                        case Operations.Minus:
                            firstNumber -= secondNumber;
                            break;
                        case Operations.Multiply:
                            firstNumber *= secondNumber;
                            break;
                        case Operations.Divide:
                            if (Double.IsNaN(firstNumber / secondNumber))
                            {
                                error = Errors.UndefinedResult;

                            }
                            else if (Double.IsInfinity(firstNumber / secondNumber))
                            {
                                error = Errors.DivideByZero;
                            }
                            else
                            {
                                firstNumber /= secondNumber;
                            }
                            break;
                    }
                }
                if (error == Errors.None)
                {
                    switch (button.Text)
                    {
                        case "+":
                            operation = Operations.Plus;
                            break;
                        case "-":
                            operation = Operations.Minus;
                            break;
                        case "*":
                            operation = Operations.Multiply;
                            break;
                        case "/":
                            operation = Operations.Divide;
                            break;
                    }
                    state = States.OperationAssigned;
                    txtDisplay.Text = formatResult(firstNumber.ToString());
                    firstNumber = parseNumber(txtDisplay.Text);
                    secondNumber = 0;
                }
                else
                {
                    txtDisplay.Text = displayErrorText(error);
                }

            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            beginningState();
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                if (state == States.OperationAssigned)
                {
                    secondNumber = parseNumber(txtDisplay.Text);
                }
                switch (operation)
                {
                    case Operations.Plus:
                        firstNumber += secondNumber;
                        break;
                    case Operations.Minus:
                        firstNumber -= secondNumber;
                        break;
                    case Operations.Multiply:
                        firstNumber *= secondNumber;
                        break;
                    case Operations.Divide:
                        if (Double.IsNaN(firstNumber / secondNumber))
                        {
                            error = Errors.UndefinedResult;
                        
                        }
                        else if (Double.IsInfinity(firstNumber / secondNumber))
                        {
                            error = Errors.DivideByZero;                            
                        }
                        else
                        {
                            firstNumber /= secondNumber;
                        }
                        break;
                    default:
                        break;
                }                             
                if (error == Errors.None)
                {
                    state = States.ResultCalculated;
                    txtDisplay.Text = formatResult(firstNumber.ToString());
                    firstNumber = parseNumber(txtDisplay.Text);
                }
                else
                {
                    txtDisplay.Text = displayErrorText(error);
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnPoint_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                if (!txtDisplay.Text.Contains('.'))
                {
                    if (state != States.OperationAssigned)
                    {
                        if (state != States.ResultCalculated)
                        {
                            txtDisplay.Text = formatResult(firstNumber.ToString()) + '.';
                        }
                        else
                        {
                            txtDisplay.Text = "0.";
                            state = States.NumberEntry;
                        }
                        firstNumber = parseNumber(txtDisplay.Text);
                    }
                    else
                    {
                        txtDisplay.Text = formatResult(secondNumber.ToString()) + '.';
                        secondNumber = parseNumber(txtDisplay.Text);
                    }
                }
                else
                {
                    if (state == States.ResultCalculated)
                    {
                        txtDisplay.Text = "0.";
                        state = States.NumberEntry;
                    }
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnChangeSign_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                if (txtDisplay.Text.Contains('-'))
                {
                    txtDisplay.Text = txtDisplay.Text.TrimStart('-');
                }
                else
                {
                    if (txtDisplay.Text != "0")
                    {
                        txtDisplay.Text = '-' + txtDisplay.Text;
                    }
                }
                if (state != States.OperationAssigned)
                {
                    firstNumber = parseNumber(formatResult(txtDisplay.Text));
                }
                else
                {
                    secondNumber = parseNumber(formatResult(txtDisplay.Text));
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                if (state != States.OperationAssigned)
                {
                    firstNumber /= 100;
                    txtDisplay.Text = formatResult(firstNumber.ToString());
                    firstNumber = parseNumber(txtDisplay.Text);
                    state = States.ResultCalculated;
                }
                else
                {
                    secondNumber = parseNumber(txtDisplay.Text);
                    secondNumber /= 100;
                    txtDisplay.Text = formatResult(secondNumber.ToString());
                    secondNumber = parseNumber(txtDisplay.Text);
                    state = States.ResultCalculated | States.OperationAssigned;
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }

        private void btnSqrt_Click(object sender, EventArgs e)
        {
            if (error == Errors.None)
            {
                if (state != States.OperationAssigned)
                {
                    if (!Double.IsNaN(Math.Sqrt(firstNumber)))
                    {
                        firstNumber = Math.Sqrt(firstNumber);
                        txtDisplay.Text = formatResult(firstNumber.ToString());
                        firstNumber = parseNumber(txtDisplay.Text);
                        state = States.ResultCalculated;
                    }
                    else
                    {
                        error = Errors.InvalidInput;
                        txtDisplay.Text = displayErrorText(error);
                    }
                }
                else
                {
                    secondNumber = parseNumber(txtDisplay.Text);
                    if (!Double.IsNaN(Math.Sqrt(secondNumber)))
                    {
                        secondNumber = Math.Sqrt(secondNumber);
                        txtDisplay.Text = formatResult(secondNumber.ToString());
                        secondNumber = parseNumber(txtDisplay.Text);
                        state = States.ResultCalculated | States.OperationAssigned;
                    }
                    else
                    {
                        error = Errors.InvalidInput;
                        txtDisplay.Text = displayErrorText(error);
                    }
                }
            }
            else
            {
                txtDisplay.Text = displayErrorText(error);
            }
        }
        private string formatResult(string result)
        {

            if (result.Contains('.'))
            {            
                result.TrimEnd('0').TrimEnd('.');

            }
            return result;
        }

        private double parseNumber(string textNumber)
        {
            if (textNumber.Contains('-'))
            {
                return double.Parse(textNumber, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint);
            }
            else
            {
                return double.Parse(textNumber);
            }
        }

        private string displayErrorText(Errors e)
        {
            if (e == Errors.DivideByZero)
                return "Cannot divide by zero";
            else if (e == Errors.InvalidInput)
                return "Invalid input";
            else if (e == Errors.UndefinedResult)
                return "Undefined result";
            else
                return "";
        }
        private void beginningState()
        {
            firstNumber = 0;
            secondNumber = 0;
            txtDisplay.Text = formatResult(firstNumber.ToString());
            state = States.NumberEntry;
            operation = Operations.None;
            error = Errors.None;
        }
    }
}
