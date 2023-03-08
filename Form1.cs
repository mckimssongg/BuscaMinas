using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuscaMinas
{
    public partial class Form1 : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int MINES = 15;
        private bool isFirstClick = true;

        private readonly bool[,] mineField = new bool[ROWS, COLS];
        private readonly Button[,] buttonField = new Button[ROWS, COLS];
        private readonly int[,] adjacentMines = new int[ROWS, COLS];
        public Form1()
        {
            Console.WriteLine(mineField);
            InitializeComponent();

            // Configurar TableLayoutPanel
            tableLayoutPanel1.ColumnCount = COLS;
            tableLayoutPanel1.RowCount = ROWS;
            tableLayoutPanel1.AutoSize = true;
            //Establecer la propiedad FormBorderStyle
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Establecer la propiedad MaximizeBox
            this.MaximizeBox = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Generar botones y agregarlos al TableLayoutPanel
            Enumerable.Range(0, ROWS).ToList().ForEach(row => {
                Enumerable.Range(0, COLS).ToList().ForEach(col => {
                    Console.WriteLine(row.ToString() + "  " + col.ToString());
                    Button button = new Button();
                    button.Height = 30; // Cambia la altura del botón a 30 píxeles
                    button.Width = 30;  // Cambia el ancho del botón a 30 píxeles
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0);
                    button.Tag = new Tuple<int, int>(row, col);
                    button.Click += Button_Click;
                    buttonField[row, col] = button;
                });
            });

            tableLayoutPanel1.Controls.AddRange(buttonField.Cast<Control>().ToArray());

            Console.WriteLine("FIN");
            TimeSpan elapsedTime = stopwatch.Elapsed;
            double seconds = elapsedTime.TotalSeconds;
            Console.WriteLine("Tiempo transcurrido: " + seconds.ToString() + " segundos");
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Tuple<int, int> position = (Tuple<int, int>)button.Tag;
            int row = position.Item1;
            int col = position.Item2;

            // Comprobar si es el primer clic
            if (isFirstClick)
            {
                // Generar minas aleatoriamente en el campo minado
                Random random = new Random();
                int count = 0;
                while (count < MINES)
                {
                    int r = random.Next(ROWS);
                    int c = random.Next(COLS);
                    //if (!mineField[r, c] && (r != row || c != col)) // No poner mina en el primer clic
                    if (!mineField[r, c] && !(Math.Abs(row - r) <= 1 && Math.Abs(col - c) <= 1))
                    {
                        mineField[r, c] = true;
                        count++;
                    }
                }

                // Calcular el número de minas adyacentes
                CalculateAdjacentMines();
                isFirstClick = false;

                // De momento vemos las minas al inicio solo para testear que todo este correcto 
                ShowMines();

            }
            //Comprobar si cae en una mina
            if (mineField[row, col])
            {
                button.Text = "X";
                button.BackColor = Color.Red;
                MessageBox.Show("Juego perdido");
                ShowMines();
            }
            else
            {
                // Mostrar el número de minas adyacentes
                int mines = adjacentMines[row, col];
                button.Text = mines.ToString();

                if (mines == 0)
                {
                    ShowAdjacentEmptyCells(row, col);
                }

            }
            if (CheckWin())
            {
                MessageBox.Show("Juego ganado");
            }
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (!mineField[row, col])
                    {
                        // Reiniciar el contador de minas adyacentes
                        int count = 0;

                        // Contar el número de minas adyacentes
                        for (int r = row - 1; r <= row + 1; r++)
                        {
                            for (int c = col - 1; c <= col + 1; c++)
                            {
                                if (r >= 0 && r < ROWS && c >= 0 && c < COLS && (r != row || c != col))
                                {
                                    if (mineField[r, c])
                                    {
                                        count++;
                                    }
                                }
                            }
                        }

                        adjacentMines[row, col] = count;
                    }
                }
            }
        }

        private bool CheckWin()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (!mineField[row, col] && buttonField[row, col].Text == "")
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //Mostrar las minas que habian en el tablero
        private void ShowMines()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (mineField[row, col])
                    {
                        buttonField[row, col].Text = "X";
                    }
                }
            }
        }
        private void ShowAdjacentEmptyCells(int row, int col)
        {
            // Verificar si la celda actual es un valor 0
            if (adjacentMines[row, col] == 0)
            {
                // Mostrar la celda actual
                buttonField[row, col].Text = adjacentMines[row, col].ToString();

                // Llamar a ShowAdjacentEmptyCells en todas las celdas adyacentes
                if (row > 0 && buttonField[row - 1, col].Text == "")
                    ShowAdjacentEmptyCells(row - 1, col);
                if (row < ROWS - 1 && buttonField[row + 1, col].Text == "")
                    ShowAdjacentEmptyCells(row + 1, col);
                if (col > 0 && buttonField[row, col - 1].Text == "")
                    ShowAdjacentEmptyCells(row, col - 1);
                if (col < COLS - 1 && buttonField[row, col + 1].Text == "")
                    ShowAdjacentEmptyCells(row, col + 1);
            }
            else
            {
                // Mostrar el valor de la celda
                buttonField[row, col].Text = adjacentMines[row, col].ToString();
            }
        }
    }
}
