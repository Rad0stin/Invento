using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Invento
{
    public partial class CashierCustomersForm : UserControl
    {
        private FlowLayoutPanel customersContainer;
        private const int CARD_WIDTH = 250;
        private const int CARD_HEIGHT = 180;
        private const int CONTAINER_PADDING = 30;
        private const int CARD_MARGIN = 15;
        private const int BOTTOM_SPACING = 100;  

        public CashierCustomersForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);
            InitializeCustomLayout();
            displayCustomers();
        }

        private void InitializeCustomLayout()
        {
            customersContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(CONTAINER_PADDING,
                                    CONTAINER_PADDING,
                                    CONTAINER_PADDING,
                                    CONTAINER_PADDING + BOTTOM_SPACING),
                AutoScrollMargin = new Size(0, BOTTOM_SPACING)
            };

            Controls.Add(customersContainer);
        }

        private class RoundedPanel : Panel
        {
            private readonly int radius = 15;
            private GraphicsPath borderPath;

            public RoundedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.Opaque,
                    true
                );
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                base.OnSizeChanged(e);
                RecreateRegion();
            }

            private void RecreateRegion()
            {
                borderPath?.Dispose();
                borderPath = new GraphicsPath();
                borderPath.AddArc(0, 0, radius, radius, 180, 90);
                borderPath.AddArc(Width - radius, 0, radius, radius, 270, 90);
                borderPath.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
                borderPath.AddArc(0, Height - radius, radius, radius, 90, 90);
                borderPath.CloseFigure();
                this.Region = new Region(borderPath);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(BackColor);

                if (borderPath != null)
                {
                    using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                    {
                        e.Graphics.DrawPath(pen, borderPath);
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    borderPath?.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        private Panel CreateCustomerCard(dynamic customer)
        {
            var card = new RoundedPanel
            {
                Width = CARD_WIDTH,
                Height = CARD_HEIGHT,
                Margin = new Padding(CARD_MARGIN),
                BackColor = Color.White
            };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.Transparent
            };

            DateTime dateValue;
            string formattedDate = DateTime.TryParse(customer.Date.ToString(), out dateValue)
                ? dateValue.ToString("MM/dd/yyyy HH:mm")
                : customer.Date.ToString();

            var idLabel = new Label
            {
                Text = $"ID: {customer.CustomerID}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, 8),
                BackColor = Color.Transparent
            };

            var dateLabel = new Label
            {
                Text = formattedDate,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(120, 8), 
                BackColor = Color.Transparent
            };

            headerPanel.Controls.AddRange(new Control[] { idLabel, dateLabel });

            var contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(10),
                BackColor = Color.Transparent,
                AutoSize = true
            };

            for (int i = 0; i < 3; i++)
            {
                contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            }

            string[] headers = { "Total Price", "Amount", "Change" };
            var values = new[] { customer.TotalPrice, customer.Amount, customer.Change };

            for (int i = 0; i < headers.Length; i++)
            {
                var headerLabel = new Label
                {
                    Text = headers[i],
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Fill
                };

                var valueLabel = new Label
                {
                    Text = $"${values[i]:N2}",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = i == 2 && Convert.ToDecimal(values[i]) > 0
                        ? Color.FromArgb(22, 163, 74)
                        : Color.Black,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Fill
                };

                contentPanel.Controls.Add(headerLabel, i, 0);
                contentPanel.Controls.Add(valueLabel, i, 1);
            }

            card.Controls.AddRange(new Control[] { contentPanel, headerPanel });
            return card;
        }

        public void displayCustomers()
        {
            customersContainer.SuspendLayout();
            customersContainer.Controls.Clear();

            CustomersData cData = new CustomersData();
            var listData = cData.allCustomers();

            foreach (var customer in listData)
            {
                customersContainer.Controls.Add(CreateCustomerCard(customer));
            }

            customersContainer.ResumeLayout();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayCustomers();
        }
    }
}