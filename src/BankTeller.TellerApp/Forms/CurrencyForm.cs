using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;

namespace BankTeller.TellerApp.Forms;

/// <summary>
/// Валютын ханш харах болон өөрчлөх форм.
/// Теллер энэ формоор валютын авах/зарах ханшийг шинэчилнэ.
/// </summary>
public class CurrencyForm : Form
{
    // ── Services ────────────────────────────────────────────────
    private readonly ICurrencyService _currencyService;

    // ── Controls ────────────────────────────────────────────────
    private DataGridView _grid;
    private Button _btnSave;
    private Button _btnRefresh;
    private Label _lblStatus;

    // ── Constructor ─────────────────────────────────────────────

    /// <summary>
    /// CurrencyForm-ийг эхлүүлж валютын ханшийг ачаална.
    /// </summary>
    /// <param name="currencyService">Валютын ханш унших/бичих сервис.</param>
    public CurrencyForm(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
        InitializeComponents();
        _ = LoadRatesAsync();
    }

    // ── UI Setup ────────────────────────────────────────────────

    /// <summary>
    /// Формын бүх UI элементүүдийг үүсгэж байрлуулна.
    /// </summary>
    private void InitializeComponents()
    {
        Text = "Валютын ханш өөрчлөх";
        Size = new Size(500, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        BackColor = Color.FromArgb(245, 247, 250);
        MaximizeBox = false;

        // ── Header ──────────────────────────────────────────────
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = Color.FromArgb(26, 60, 110)
        };
        header.Controls.Add(new Label
        {
            Text = "💱  Валютын ханш",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0)
        });

        // ── Grid ────────────────────────────────────────────────
        _grid = new DataGridView
        {
            Location = new Point(20, 76),
            Size = new Size(444, 200),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 10),
            GridColor = Color.FromArgb(220, 225, 235),
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            EnableHeadersVisualStyles = false
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 60, 110);
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 255);
        _grid.DefaultCellStyle.SelectionForeColor = Color.Black;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Валют",
            DataPropertyName = "CurrencyCode",
            Width = 90,
            ReadOnly = true
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Авах ₮",
            DataPropertyName = "BuyRate",
            Width = 174
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Зарах ₮",
            DataPropertyName = "SellRate",
            Width = 174
        });

        // ── Status label ────────────────────────────────────────
        _lblStatus = new Label
        {
            Location = new Point(20, 284),
            Size = new Size(444, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            Text = "Ачааллаж байна..."
        };

        // ── Refresh button ───────────────────────────────────────
        _btnRefresh = new Button
        {
            Text = "🔄  Шинэчлэх",
            Location = new Point(20, 312),
            Size = new Size(140, 38),
            BackColor = Color.FromArgb(230, 235, 245),
            ForeColor = Color.FromArgb(26, 60, 110),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(180, 190, 210);
        _btnRefresh.Click += async (s, e) => await LoadRatesAsync();

        // ── Save button ──────────────────────────────────────────
        _btnSave = new Button
        {
            Text = "💾  Хадгалах",
            Location = new Point(324, 312),
            Size = new Size(140, 38),
            BackColor = Color.FromArgb(26, 60, 110),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnSave.FlatAppearance.BorderSize = 0;
        _btnSave.Click += async (s, e) => await SaveRatesAsync();

        Controls.AddRange(new Control[]
        {
            header, _grid, _lblStatus, _btnRefresh, _btnSave
        });
    }

    // ── Data ────────────────────────────────────────────────────

    /// <summary>
    /// Серверээс бүх валютын ханшийг татаж grid-д харуулна.
    /// </summary>
    private async Task LoadRatesAsync()
    {
        _btnRefresh.Enabled = false;
        _lblStatus.Text = "Ачааллаж байна...";
        _lblStatus.ForeColor = Color.Gray;

        try
        {
            var rates = await _currencyService.GetAllRatesAsync();
            _grid.DataSource = null;
            _grid.DataSource = rates;
            _lblStatus.Text = $"{rates.Count} валют ачааллагдлаа  •  {DateTime.Now:HH:mm:ss}";
            _lblStatus.ForeColor = Color.Gray;
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Алдаа: {ex.Message}";
            _lblStatus.ForeColor = Color.Crimson;
        }
        finally
        {
            _btnRefresh.Enabled = true;
        }
    }

    /// <summary>
    /// Grid-д байгаа бүх мөрийн ханшийг API-д хадгалуулна.
    /// </summary>
    private async Task SaveRatesAsync()
    {
        _btnSave.Text = "Хадгалж байна...";
        _btnSave.Enabled = false;
        _lblStatus.Text = "Хадгалж байна...";
        _lblStatus.ForeColor = Color.DarkOrange;

        try
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                var code = row.Cells[0].Value?.ToString() ?? "";
                var buyRate = decimal.TryParse(row.Cells[1].Value?.ToString(), out var b) ? b : 0;
                var sellRate = decimal.TryParse(row.Cells[2].Value?.ToString(), out var s) ? s : 0;

                await _currencyService.UpdateRateAsync(code, buyRate, sellRate);
            }

            _lblStatus.Text = $"✓  Амжилттай хадгаллаа  •  {DateTime.Now:HH:mm:ss}";
            _lblStatus.ForeColor = Color.SeaGreen;

            MessageBox.Show("Валютын ханш амжилттай шинэчлэгдлээ.",
                "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            _lblStatus.Text = $"Алдаа: {ex.Message}";
            _lblStatus.ForeColor = Color.Crimson;
        }
        finally
        {
            _btnSave.Text = "💾  Хадгалах";
            _btnSave.Enabled = true;
        }
    }
}