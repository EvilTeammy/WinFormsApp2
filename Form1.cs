namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        //начальный массив карты
        private bool[,] World = new bool[10, 10] {{ false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false },
                                          { false, false, false, false, false, false, false, false, false, false }};

        //Конструктор класса, где прогружаются компонениы UI
        public Form1()
        {
            InitializeComponent();
            for(int i = 0; i < 10; i++)
            {
                dataGridView1.Columns.Add(i.ToString(), i.ToString());
                dataGridView1.Columns[i].Width = 40;
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 40;
            }
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(World[i, j]) dataGridView1[j, i].Style.BackColor = Color.White;
                    else dataGridView1[j, i].Style.BackColor = Color.Black;
                }
            }
        }

        //Обработка одного шага (вызывается по кнопке или по таймеру)
        private void Step()
        {
            int CountOfLive = 0;

            bool[,] worldVar = (bool[,])World.Clone();

            for(int i = 0; i < World.GetLength(0); i++)
            {
                for(int j = 0; j < World.GetLength(0); j++)
                {

                    int a = i - 1;
                    if(a < 0) a = World.GetLength(0) - 1;

                    int b = j - 1;
                    if(b < 0) b = World.GetLength(0) - 1;

                    int c = i + 1;
                    if(c > 9) c = 0;

                    int d = j + 1;
                    if(d > 9) d = 0;


                    if(World[a, b]) { CountOfLive++; }
                    if(World[i, b]) { CountOfLive++; }
                    if(World[c, b]) { CountOfLive++; }
                    if(World[a, j]) { CountOfLive++; }
                    if(World[c, j]) { CountOfLive++; }
                    if(World[a, d]) { CountOfLive++; }
                    if(World[i, d]) { CountOfLive++; }
                    if(World[c, d]) { CountOfLive++; }

                    if(!World[i, j] && CountOfLive == 3) { worldVar[i, j] = true; }
                    else if(!(World[i, j] && (CountOfLive == 2 || CountOfLive == 3))) { worldVar[i, j] = false; }

                    CountOfLive = 0;
                }
            }

            World = worldVar;

            for(int i = 0; i < World.GetLength(0); i++)
            {
                for(int j = 0; j < World.GetLength(0); j++)
                {
                    if(World[i, j]) dataGridView1[j, i].Style.BackColor = Color.White;
                    else dataGridView1[j, i].Style.BackColor = Color.Black;
                }
            }
        }

        //Обработчик нажатия кнопки Tick (Просто вызывает функцию которая обрабатывает один ход)
        private void Button1_Click(object sender, EventArgs e)
        {
            Step();
        }

        //Сохранение карты в файл
        private async void Button2_ClickAsync(object sender, EventArgs e)
        {
            string outText = "";
            for(int i = 0; i < World.GetLength(0); i++)
            {
                if(i != 0)
                    outText += "\n";
                for(int j = 0; j < World.GetLength(0); j++)
                {
                    if(j != 0)
                        outText += " ";
                    if(World[i, j])
                        outText += 1;
                    else outText += 0;
                }
            }
            MessageBox.Show(outText);

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*"
            };

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using StreamWriter writer = new(saveFileDialog.FileName, false);
                await writer.WriteAsync(outText);
            }
        }

        //Очистка выделения на DataGridView после загрузки окна
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        //Загрузка карты из файла
        private void Button3_Click(object sender, EventArgs e)
        {
            bool[,] inArr;
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*",
            };
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using StreamReader reader = new(openFileDialog.FileName);
                string inText = reader.ReadToEnd();
                string[] strings = inText.Split('\n');
                inArr = new bool[strings.Length, strings.Length];
                for(int i = 0; i < strings.Length; i++)
                {
                    string[] line = strings[i].Split(' ');
                    for(int j = 0; j < line.Length; j++)
                    {
                        if(line[j][0].Equals('0')) inArr[i, j] = false;
                        else if(line[j][0].Equals('1')) inArr[i, j] = true;
                    }
                }
                World = inArr;
                DataGridReDraw();
            }
        }

        //Перерисовка DataGridView при изменеии карты
        private void DataGridReDraw()
        {
            int len = dataGridView1.Columns.Count;
            for(int i = 0; i < len; i++)
            {
                dataGridView1.Rows.RemoveAt(0);
                dataGridView1.Columns.RemoveAt(0);
            }

            for(int i = 0; i < World.GetLength(0); i++)
            {
                dataGridView1.Columns.Add(i.ToString(), i.ToString());
                dataGridView1.Rows.Add();
                dataGridView1.Columns[i].Width = Convert.ToInt32(numericUpDown1.Value);
                dataGridView1.Rows[i].Height = Convert.ToInt32(numericUpDown1.Value);
            }

            for(int i = 0; i < World.GetLength(0); i++)
            {
                for(int j = 0; j < World.GetLength(0); j++)
                {
                    if(World[i, j]) dataGridView1[j, i].Style.BackColor = Color.White;
                    else dataGridView1[j, i].Style.BackColor = Color.Black;
                }
            }
            dataGridView1.ClearSelection();
        }

        //Изменение размера матрицы карты
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int size = Convert.ToInt32(((NumericUpDown)sender).Value);
            World = ResizeArray(ref World, size, size);
            DataGridReDraw();
        }

        //Изменение размера ячеек
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < World.GetLength(0); i++)
            {
                dataGridView1.Columns[i].Width = Convert.ToInt32(((NumericUpDown)sender).Value);
                dataGridView1.Rows[i].Height = Convert.ToInt32(((NumericUpDown)sender).Value);
            }
        }

        //Функция таймера которая выполняется каждый тик и вызывает обработку шага
        private void timer1_Tick(object sender, EventArgs e)
        {
            Step();
        }

        //Включение или отключение таймера по чекбоксу
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(((CheckBox)sender).Checked)
            {
                timer1.Start();
                numericUpDown3.Enabled = true;
                label3.Enabled = true;
            }
            else
            {
                timer1.Stop();
                numericUpDown3.Enabled = false;
                label3.Enabled = false;
            }
        }

        //Изменение времени между тиками по NumericUpDown
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)((NumericUpDown)sender).Value;
        }

        //Функция для изменения размера матрицы
        private static T[,] ResizeArray<T>(ref T[,] array, int size1, int size2)
        {
            T[,] new_array = new T[size1, size2];
            size1 = Math.Min(array.GetLength(0), size1);
            size2 = Math.Min(array.GetLength(1), size2);
            for(int i = 0; i < size1; i++)
            {
                for(int j = 0; j < size2; j++) new_array[i, j] = array[i, j];
            }
            return new_array;
        }
    }
}