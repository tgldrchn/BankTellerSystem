namespace BankTeller.NumberTerminal;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblTicketNumber = new Label();
        btnIssueTicket = new Button();
        lblMessage = new Label();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblTitle.Location = new Point(414, 60);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(639, 46);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "БАНКНЫ ДУГААР ОЛГОХ ТЕРМИНАЛ";
        lblTitle.TextAlign = ContentAlignment.TopCenter;
        // 
        // lblTicketNumber
        // 
        lblTicketNumber.AutoSize = true;
        lblTicketNumber.Font = new Font("Segoe UI", 64.2000046F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblTicketNumber.Location = new Point(638, 106);
        lblTicketNumber.Name = "lblTicketNumber";
        lblTicketNumber.Size = new Size(194, 145);
        lblTicketNumber.TabIndex = 1;
        lblTicketNumber.Text = "---";
        lblTicketNumber.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnIssueTicket
        // 
        btnIssueTicket.Font = new Font("Segoe UI", 22.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
        btnIssueTicket.Location = new Point(541, 340);
        btnIssueTicket.Name = "btnIssueTicket";
        btnIssueTicket.Size = new Size(368, 60);
        btnIssueTicket.TabIndex = 2;
        btnIssueTicket.Text = "Дугаар авах";
        btnIssueTicket.TextAlign = ContentAlignment.BottomCenter;
        btnIssueTicket.UseVisualStyleBackColor = true;
        // 
        // lblMessage
        // 
        lblMessage.AutoSize = true;
        lblMessage.Font = new Font("Segoe UI", 12F);
        lblMessage.Location = new Point(583, 299);
        lblMessage.Name = "lblMessage";
        lblMessage.Size = new Size(289, 28);
        lblMessage.TabIndex = 3;
        lblMessage.Text = "Дугаар авах товчийг дарна уу.";
        lblMessage.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1509, 450);
        Controls.Add(lblMessage);
        Controls.Add(btnIssueTicket);
        Controls.Add(lblTicketNumber);
        Controls.Add(lblTitle);
        Name = "Form1";
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTitle;
    private Label lblTicketNumber;
    private Button btnIssueTicket;
    private Label lblMessage;
}
