using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        private const int limitFlags = MINES + 3;
        private bool isFirstClick = true;
        private Label tituloForm = new Label();

        private readonly bool[,] mineField = new bool[ROWS, COLS];
        private readonly Button[,] buttonField = new Button[ROWS, COLS];
        private readonly int[,] adjacentMines = new int[ROWS, COLS];
        private readonly bool[,] flagsField = new bool[ROWS, COLS];
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // ACA TOMAREMOS EL NIVEL DE DIFICULTAD CON UN FORM
            // Instanciamos una el form de la dificultad 
            tituloForm.Text = "Selecciona el nivel de dificultad";
            tituloForm.Font = new Font("Arial", 20, FontStyle.Bold);
            tituloForm.Width = 432;
            tituloForm.Height = 40;
            int centerX = (this.ClientSize.Width - tituloForm.Width) / 2;
            tituloForm.Location = new Point(centerX, 0);
            this.Controls.Add(tituloForm);

            for (int i = 0; i < 3; i++)
            {
                int level = i + 1;
                Button button_nivel = new Button();
                button_nivel.Tag = level;
                button_nivel.Height = 40;
                button_nivel.Width = 150;
                button_nivel.Text = "Nivel " + level.ToString();
                button_nivel.Font = new Font("Arial", 20, FontStyle.Bold);
                button_nivel.Margin = new Padding(0);
                int centerXButtonLevels = (this.ClientSize.Width - button_nivel.Width) / 2;
                int centerYButtonLevels  = level * button_nivel.Height;
                button_nivel.Location = new Point(centerXButtonLevels, centerYButtonLevels);
                button_nivel.Click += button_nivel_Click;
                this.Controls.Add(button_nivel);
                

            }
            
            //InitGameLayoutPanel(0,0,0);
        }

        private void button_nivel_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int nivel = (int) button.Tag;

            switch (nivel)
            {
                case 1:
                    InitGameLayoutPanel
                        (
                             10, 10,8
                        );
                    break;

                case 2:
                    InitGameLayoutPanel
                        (
                             20, 20, 10
                        );
                    break;
                case 3:
                    InitGameLayoutPanel
                         (
                              25, 25, 15
                         );
                    break;
            }
        }

            private void InitGameLayoutPanel(int rows, int cols, int mines)
        {
            // Configurar TableLayoutPanel
            tableLayoutPanel1.ColumnCount = COLS;
            tableLayoutPanel1.RowCount = ROWS;
            tableLayoutPanel1.AutoSize = true;
            //Establecer la propiedad FormBorderStyle
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Establecer la propiedad MaximizeBox
            this.Size = new Size(820, 500);
            //this.MaximizeBox = false;

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            // Generar botones y agregarlos al TableLayoutPanel
            Enumerable.Range(0, ROWS).ToList().ForEach(row => {
                Enumerable.Range(0, COLS).ToList().ForEach(col => {
                    Console.WriteLine(row.ToString() + "  " + col.ToString());
                    Button button = new Button();
                    button.Height = 40; // Cambia la altura del botón a 30 píxeles
                    button.Width = 40;  // Cambia el ancho del botón a 30 píxeles
                    button.Dock = DockStyle.Fill;
                    button.Font = new Font("Arial", 20, FontStyle.Bold);
                    button.Margin = new Padding(0);
                    button.Tag = new Tuple<int, int>(row, col);
                    button.MouseUp += button_MouseButtons;
                    buttonField[row, col] = button;
                });
            });

            tableLayoutPanel1.Controls.AddRange(buttonField.Cast<Control>().ToArray());

            ResizeLayout();

            //Console.WriteLine("FIN");
            //TimeSpan elapsedTime = stopwatch.Elapsed;
            //double seconds = elapsedTime.TotalSeconds;
            //Console.WriteLine("Tiempo transcurrido: " + seconds.ToString() + " segundos");
            tituloForm.Hide();
        }

        private void button_MouseButtons(object sender, EventArgs e)
        {
            // tomar el click izquierdo
            MouseEventArgs me = (MouseEventArgs)e;
            Console.WriteLine(me.Button.ToString());

            if (me.Button == MouseButtons.Left)
            {
                Button_Click_Left(sender, e);
            }

            // tomar el click derecho
            if (me.Button == MouseButtons.Right)
            {
                // si no es el primer click
                if (isFirstClick)
                {
                    // si es el primer click
                    MessageBox.Show("Para iniciar el juego debes de\n hacer click izquierdo en un campo");
                }
                else
                {
                    Button button = (Button)sender;
                    Tuple<int, int> position = (Tuple<int, int>)button.Tag;
                    int row = position.Item1;
                    int col = position.Item2;

                    // hacer click en la celda
                    if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
                    {
                        buttonField[row, col].Text = "🚩";
                        buttonField[row, col].ForeColor = Color.Green;
                    }
                }
                
            }
        }
        private void Button_Click_Left(object sender, EventArgs e)
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
                //ShowMines();

            }
            //Comprobar si cae en una mina
            if (mineField[row, col])
            {
                button.Text = "💣";
                button.ForeColor = Color.Black;
                button.BackColor = Color.Red;
                MessageBox.Show("Juego perdido");
                ShowMines();
            }
            else
            {
                // Mostrar el número de minas adyacentes
                int mines = adjacentMines[row, col];
                button.Text = mines.ToString();
                button.ForeColor = Color.Black;

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
                        buttonField[row, col].Text = "💣";
                        buttonField[row, col].ForeColor = Color.Black;
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
        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeLayout();
        }
        private void ResizeLayout()
        {
            // Calcula la posición X para centrar el TableLayoutPanel
            int centerX = (this.ClientSize.Width - tableLayoutPanel1.Width) / 2;

            // Calcula la posición Y para centrar el TableLayoutPanel
            int centerY = (this.ClientSize.Height - tableLayoutPanel1.Height) / 2;

            // Establece la posición del TableLayoutPanel
            tableLayoutPanel1.Location = new Point(centerX, centerY);

            int centerXtitulo = (this.ClientSize.Width - this.tituloForm.Width) / 2;
            tituloForm.Location = new Point(centerXtitulo, 0);

        }
    }
}