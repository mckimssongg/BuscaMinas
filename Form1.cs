using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BuscaMinas
{
    public partial class Form1 : Form
    {
        private int ROWS;
        private int COLS;
        private int MINES;
        private bool isFirstClick = true;
        private Label tituloForm = new Label();
        private int limitFlags;
        private int minesHide = 0;

        private bool[,] mineField;
        private Button[,] buttonField;
        private int[,] adjacentMines;
        private bool[,] flagsField;
        private List<Button> buttons_nivels = new List<Button>();

        //private  bool[,] mineField = new bool[ROWS, COLS];
        //private Button[,] buttonField = new Button[ROWS, COLS];
        //private  int[,] adjacentMines = new int[ROWS, COLS];
        //private  bool[,] flagsField = new bool[ROWS, COLS];

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
                buttons_nivels.Add(button_nivel);
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
                             10, 10,15
                        );
                    break;

                case 2:
                    InitGameLayoutPanel
                        (
                             12, 12, 10
                        );
                    break;
                case 3:
                    InitGameLayoutPanel
                         (
                              14, 14, 12
                         );
                    break;
            }
        }

        private void InitGameLayoutPanel(int rows, int cols, int mines)
        {
            this.COLS = cols;
            this.ROWS = rows;
            this.MINES = mines;
            this.limitFlags = mines + 3;
            this.mineField = new bool[ROWS, COLS];
            this.buttonField = new Button[ROWS, COLS];
            this.adjacentMines = new int[ROWS, COLS];
            this.flagsField = new bool[ROWS, COLS];

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
                    //Console.WriteLine(row.ToString() + "  " + col.ToString());
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
            foreach (Button btn_level in buttons_nivels)
            {
                btn_level.Visible = false;
            }
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
                Button_Click_Right(sender, e);
            }
        }
        private void Button_Click_Right(object sender, EventArgs e)
        {
            ////DEBUG DE CELDAS
            //for (int row = 0; row < ROWS; row++)
            //{
            //    for (int col = 0; col < COLS; col++)
            //    {
            //        Console.Write(adjacentMines[row, col] + " ");
            //    }
            //    Console.WriteLine();
            //}

            int totalFlags = flagsField.Cast<bool>().Count(x => x);

            Button button = (Button)sender;
            Tuple<int, int> position = (Tuple<int, int>)button.Tag;
            int row = position.Item1;
            int col = position.Item2;

            if (totalFlags >= limitFlags)
            {
                if (flagsField[row, col])
                {
                    flagsField[row, col] = false;
                    buttonField[row, col].Text = "";
                    buttonField[row, col].ForeColor = Color.Black;
                }
                else
                {
                    MessageBox.Show("Has alcanzado el límite de banderas");
                }
            }
            else
            {
                // si no es el primer click
                if (isFirstClick)
                {
                    // si es el primer click
                    MessageBox.Show("Para iniciar el juego debes de\n hacer click izquierdo en un campo");
                }
                else
                {
                    // hacer click en la celda
                    if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
                    {
                        if (flagsField[row, col])
                        {
                            flagsField[row, col] = false;
                            buttonField[row, col].Text = "";
                            buttonField[row, col].ForeColor = Color.Black;
                        }
                        else if (mineField[row, col] )
                        {
                            minesHide += 1;
                            buttonField[row, col].Text = "🚩";
                            flagsField[row, col] = true;
                            buttonField[row, col].ForeColor = Color.Green;
                        }
                        else if (buttonField[row, col].Text == "") // Verificar que la celda no tenga algo ya printeado
                        {
                            // Si la celda no tiene número, bomba, permitir colocar la bandera
                            buttonField[row, col].Text = "🚩";
                            flagsField[row, col] = true;
                            buttonField[row, col].ForeColor = Color.Green;
                        }

                        if (minesHide >= MINES)
                        {
                            MessageBox.Show("Juego ganado");
                            WinGame();
                            blockedGame();
                        }
                    }
                }
            }
        }

        private void blockedGame()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    buttonField[row, col].Enabled = false;
                }
            }
        }
        private void WinGame()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (!mineField[row, col])
                    {
                        buttonField[row, col].Text = adjacentMines[row, col].ToString();
                        buttonField[row, col].ForeColor = Color.Black;
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

            //comprobar que no exista una bandera
            if (!flagsField[row, col])
            {
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
                    button.Text = "💣";
                    button.ForeColor = Color.Black;
                    button.BackColor = Color.Red;
                    MessageBox.Show("Juego perdido");
                    ShowMines();
                    WinGame();
                    blockedGame();
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

                    if (CheckWin())
                    {
                        ShowMines();
                        WinGame();
                        blockedGame();
                        MessageBox.Show("Juego ganado");
                    }

                }
            }
           
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    // Si no hay mina vamos a agregar un numero a la casilla
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
                    if (mineField[row, col] && flagsField[row, col])
                    {
                        buttonField[row, col].Text = "🚩💣";
                        buttonField[row, col].Font = new Font("Arial", 9, FontStyle.Bold);
                        buttonField[row, col].ForeColor = Color.BlueViolet;
                    }
                    else if (mineField[row, col])
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