namespace CalculatorApp;

public partial class Form1 : Form
{
    double firstNumber = 0;
    double secondNumber = 0;
    double result = 0;
    string operation = "";

    TextBox txtDisplay;
    Label lblTitle;

    public Form1()
    {
        InitializeComponent();

        Text = "Calculator";
        Width = 350;
        Height = 500;
        StartPosition = FormStartPosition.CenterScreen;

        lblTitle = new Label();
        lblTitle.Name = "lblTitle";
        lblTitle.Text = "Calculator";
        lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(125, 20);
        Controls.Add(lblTitle);

        txtDisplay = new TextBox();
        txtDisplay.Name = "txtDisplay";
        txtDisplay.Text = "0";
        txtDisplay.ReadOnly = true;
        txtDisplay.Font = new Font("Arial", 20);
        txtDisplay.TextAlign = HorizontalAlignment.Right;
        txtDisplay.Location = new Point(25, 60);
        txtDisplay.Width = 280;
        Controls.Add(txtDisplay);

        AddButton("btn7", "7", 25, 110, NumberButton_Click);
        AddButton("btn8", "8", 95, 110, NumberButton_Click);
        AddButton("btn9", "9", 165, 110, NumberButton_Click);
        AddButton("btnDivide", "÷", 235, 110, OperatorButton_Click);

        AddButton("btn4", "4", 25, 170, NumberButton_Click);
        AddButton("btn5", "5", 95, 170, NumberButton_Click);
        AddButton("btn6", "6", 165, 170, NumberButton_Click);
        AddButton("btnMultiply", "x", 235, 170, OperatorButton_Click);

        AddButton("btn1", "1", 25, 230, NumberButton_Click);
        AddButton("btn2", "2", 95, 230, NumberButton_Click);
        AddButton("btn3", "3", 165, 230, NumberButton_Click);
        AddButton("btnMinus", "-", 235, 230, OperatorButton_Click);

        AddButton("btn0", "0", 25, 290, NumberButton_Click);
        AddButton("btnDecimal", ".", 95, 290, NumberButton_Click);
        AddButton("btnClear", "C", 165, 290, btnClear_Click);
        AddButton("btnPlus", "+", 235, 290, OperatorButton_Click);

        AddButton("btnEquals", "=", 25, 350, btnEquals_Click);
    }

    private void AddButton(
        string name,
        string text,
        int x,
        int y,
        EventHandler clickHandler)
    {
        Button button = new Button();

        button.Name = name;
        button.Text = text;
        button.Font = new Font("Arial", 14);
        button.Width = 65;
        button.Height = 50;
        button.Location = new Point(x, y);

        button.Click += clickHandler;

        Controls.Add(button);
    }

    private void NumberButton_Click(object sender, EventArgs e)
    {
    Button button = (Button)sender;
    if (txtDisplay.Text == "0")
    txtDisplay.Text = button.Text;
    else
    txtDisplay.Text += button.Text;
    }

    private void OperatorButton_Click(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        firstNumber = double.Parse(txtDisplay.Text);
        operation = button.Text;
        txtDisplay.Clear();
    }

    private void btnEquals_Click(object sender, EventArgs e)
    {
        try
        {
            secondNumber = double.Parse(txtDisplay.Text);
            switch (operation)
        {
            case "+": result = firstNumber + secondNumber; break;
            case "-": result = firstNumber - secondNumber; break;
            case "x": result = firstNumber * secondNumber; break;
            case "÷":
            if (secondNumber == 0)
            throw new DivideByZeroException();
            result = firstNumber / secondNumber;
            break;
        }
            txtDisplay.Text = result.ToString();
        }
            catch (Exception ex)
        {

        MessageBox.Show(ex.Message, "Error");
        }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        firstNumber = 0;
        secondNumber = 0;
        result = 0;
        operation = "";
        txtDisplay.Text = "0";
    }

    private void btnDecimal_Click(object sender, EventArgs e)
    {
        if (!txtDisplay.Text.Contains("."))
        txtDisplay.Text += ".";
    }
}