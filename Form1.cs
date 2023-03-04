using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscaMinas
{
    public partial class Form1 : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int MINES = 10;

        private bool[,] mineField = new bool[ROWS, COLS];
        private Button[,] buttonField = new Button[ROWS, COLS];

        public Form1()
        {
            InitializeComponent();

            // Configurar TableLayoutPanel
            tableLayoutPanel1.ColumnCount = COLS;
            tableLayoutPanel1.RowCount = ROWS;
            tableLayoutPanel1.AutoSize = true;

            // Generar botones y agregarlos al TableLayoutPanel
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    Button button = new Button();
                    button.Height = 30; // Cambia la altura del botón a 30 píxeles
                    button.Width = 30;  // Cambia el ancho del botón a 30 píxeles
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0);
                    button.Tag = new Tuple<int, int>(row, col);
                    //button.Click += Button_Click;
                    tableLayoutPanel1.Controls.Add(button, col, row);
                    buttonField[row, col] = button;
                    tableLayoutPanel1.GetControlFromPosition(col, row).BackColor = Color.Blue; // Dar color dependiendo del valor de la casilla
                }
            }

            // Generar minas aleatoriamente en el campo minado
            Random random = new Random();
            int count = 0;
            while (count < MINES)
            {
                int row = random.Next(ROWS);
                int col = random.Next(COLS);
                if (!mineField[row, col])
                {
                    mineField[row, col] = true;
                    count++;
                }
            }
        }















































        //private void Button_Click(object sender, EventArgs e)
        //{
        //    Button button = (Button)sender;
        //    Tuple<int, int> position = (Tuple<int, int>)button.Tag;
        //    int row = position.Item1;
        //    int col = position.Item2;

        //    if (mineField[row, col])
        //    {
        //        button.Text = "X";
        //        MessageBox.Show("Juego perdido");
        //        ShowMines();
        //    }
        //    else
        //    {
        //        int mineCount = CountMines(row, col);
        //        if (mineCount == 0)
        //        {
        //            RevealEmptyCells(row, col);
        //        }
        //        else
        //        {
        //            button.Text = mineCount.ToString();
        //        }

        //        if (CheckWin())
        //        {
        //            MessageBox.Show("Juego ganado");
        //        }
        //    }
        //}

        //private int CountMines(int row, int col)
        //{
        //    int count = 0;
        //    for (int r = row - 1; r <= row + 1; r++)
        //    {
        //        for (int c = col - 1; c <= col + 1; c++)
        //        {
        //            if (r >= 0 && r < ROWS && c >= 0 && c < COLS && mineField[r, c])
        //            {
        //                count++;
        //            }
        //        }
        //    }
        //    return count;
        //}

        //private void RevealEmptyCells(int row, int col)
        //{
        //    if (row < 0 || row >= ROWS || col < 0 || col >= COLS || buttonField[row, col].Text != "") return;

        //    int mineCount = CountMines(row, col);
        //    if (mineCount > 0)
        //    {
        //        buttonField[row, col].Text = mineCount.ToString();
        //    }
        //    else
        //    {
        //        buttonField[row, col].Text = " ";
        //        for (int r = row - 1; r <= row + 1; r++)
        //        {
        //            for (int c = col - 1; c <= col + 1; c++)
        //            {
        //                RevealEmptyCells(r, c);
        //            }
        //        }
        //    }
        //}

        //private bool CheckWin()
        //{
        //    for (int row = 0; row < ROWS; row++)
        //    {
        //        for (int col = 0; col < COLS; col++)
        //        {
        //            if (!mineField[row, col] && buttonField[row, col].Text == "")
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        //private void ShowMines()
        //{
        //    for (int row = 0; row < ROWS; row++)
        //    {
        //        for (int col = 0; col < COLS; col++)
        //        {
        //            if (mineField[row, col])
        //            {
        //                buttonField[row, col].Text = "X";
        //            }
        //        }
        //    }
        //}
    }
}
