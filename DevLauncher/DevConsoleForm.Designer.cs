using System;
using CityZero.Core;

namespace CityZero.DevLauncher
{
    partial class DevConsoleForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ticker?.Stop();
                _ticker?.Dispose();
                components?.Dispose();
                ServiceLocator.Clear();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ── Form ──────────────────────────────────────────────────────────────
            Text            = "CITY//ZERO  Dev Console";
            Size            = new System.Drawing.Size(1100, 720);
            MinimumSize     = new System.Drawing.Size(800, 500);
            BackColor       = System.Drawing.Color.FromArgb(18, 18, 24);
            ForeColor       = System.Drawing.Color.LightGray;
            Font            = new System.Drawing.Font("Cascadia Code", 9.5f,
                              System.Drawing.FontStyle.Regular,
                              System.Drawing.GraphicsUnit.Point);
            StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;

            // ── HUD strip (top) ───────────────────────────────────────────────────
            _hud = new System.Windows.Forms.Label
            {
                Dock      = System.Windows.Forms.DockStyle.Top,
                Height    = 26,
                BackColor = System.Drawing.Color.FromArgb(30, 30, 40),
                ForeColor = System.Drawing.Color.FromArgb(220, 220, 220),
                Font      = new System.Drawing.Font("Cascadia Code", 8.5f,
                            System.Drawing.FontStyle.Bold,
                            System.Drawing.GraphicsUnit.Point),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0),
                Text      = "Booting…"
            };

            // ── Output pane ───────────────────────────────────────────────────────
            _output = new System.Windows.Forms.RichTextBox
            {
                Dock            = System.Windows.Forms.DockStyle.Fill,
                ReadOnly        = true,
                BackColor       = System.Drawing.Color.FromArgb(12, 12, 18),
                ForeColor       = System.Drawing.Color.LightGray,
                Font            = new System.Drawing.Font("Cascadia Code", 9.5f,
                                  System.Drawing.FontStyle.Regular,
                                  System.Drawing.GraphicsUnit.Point),
                BorderStyle     = System.Windows.Forms.BorderStyle.None,
                ScrollBars      = System.Windows.Forms.RichTextBoxScrollBars.Vertical,
                WordWrap        = false,
                Margin          = new System.Windows.Forms.Padding(0),
            };

            // ── Input row (bottom) ────────────────────────────────────────────────
            var inputPanel = new System.Windows.Forms.Panel
            {
                Dock      = System.Windows.Forms.DockStyle.Bottom,
                Height    = 36,
                BackColor = System.Drawing.Color.FromArgb(24, 24, 34),
                Padding   = new System.Windows.Forms.Padding(4, 4, 4, 4),
            };

            _sendBtn = new System.Windows.Forms.Button
            {
                Text      = "▶",
                Dock      = System.Windows.Forms.DockStyle.Right,
                Width     = 44,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(200, 50, 50),
                ForeColor = System.Drawing.Color.White,
                Font      = new System.Drawing.Font("Segoe UI", 10f,
                            System.Drawing.FontStyle.Bold,
                            System.Drawing.GraphicsUnit.Point),
                Cursor    = System.Windows.Forms.Cursors.Hand,
            };
            _sendBtn.FlatAppearance.BorderSize = 0;
            _sendBtn.Click += (_, _) => Submit();

            _input = new System.Windows.Forms.TextBox
            {
                Dock        = System.Windows.Forms.DockStyle.Fill,
                BackColor   = System.Drawing.Color.FromArgb(30, 30, 44),
                ForeColor   = System.Drawing.Color.White,
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Font        = new System.Drawing.Font("Cascadia Code", 9.5f,
                              System.Drawing.FontStyle.Regular,
                              System.Drawing.GraphicsUnit.Point),
                PlaceholderText = "Enter command… (Enter to run, ↑↓ history)",
            };
            _input.KeyDown += InputKeyDown;

            inputPanel.Controls.Add(_input);
            inputPanel.Controls.Add(_sendBtn);

            // ── Compose ───────────────────────────────────────────────────────────
            Controls.Add(_output);
            Controls.Add(inputPanel);
            Controls.Add(_hud);

            ResumeLayout(false);
        }

        private void Submit()
        {
            Execute(_input.Text);
            _input.Clear();
        }

        private void InputKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Return:
                    e.SuppressKeyPress = true;
                    Submit();
                    break;

                case System.Windows.Forms.Keys.Up:
                    e.SuppressKeyPress = true;
                    if (_history.Count > 0)
                    {
                        _histIdx = Math.Min(_histIdx + 1, _history.Count - 1);
                        _input.Text = _history[_histIdx];
                        _input.SelectionStart = _input.Text.Length;
                    }
                    break;

                case System.Windows.Forms.Keys.Down:
                    e.SuppressKeyPress = true;
                    _histIdx = Math.Max(_histIdx - 1, -1);
                    _input.Text = _histIdx >= 0 ? _history[_histIdx] : string.Empty;
                    _input.SelectionStart = _input.Text.Length;
                    break;
            }
        }
    }
}
