using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace 自动处理数据_4
{
    public partial class Form1 : Form
    {
        private char[] splitter1 = new char[] { '\t' }, splitter2 = new char[] { ' ', '=' }, splitter3 = new char[] { '\t', ' ', '"', ';' };

        private bool selectFile1Changed = false,
            selectFile2Changed = false,
            selectFile3Changed = false,
            selectFile4Changed = false,
            selectFileLevel3Changed = false,
            selectFile5Changed = false;

        private OpenFileDialog importFile1 = null,
            importFile2 = null,
            importFile3 = null,
            importFile4 = null,
            importFileLevel3 = null,
            importFile5 = null;

        private SaveFileDialog exportFile = null;

        private FileStream file_Pseudogene = null,
            file_Promoter = null,
            file_Enhancer = null,
            file_MethyExpPosition = null,
            file_CodingGene = null;

        private StreamReader reader_Pseudogene = null,
            reader_Promoter = null,
            reader_Enhancer = null,
            reader_MethyExpPosition = null,
            reader_CodingGene = null;

        private DataTable pseudogenes = null,
            promoters = null,
            enhancers = null,
            methyExpPositions = null,
            codingGenes = null;

        private void checkBox_Promoter_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox_Promoter.Checked;
            textBox_Promoter_File.Enabled = checkBox_Promoter.Checked;
            button_SelectFile2.Enabled = checkBox_Promoter.Checked;
        }

        private void checkBox_Enhancer_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Enhancer_File.Enabled = checkBox_Enhancer.Checked;
            button_SelectFile3.Enabled = checkBox_Enhancer.Checked;
        }

        private void checkBox_MethyPosition_CheckedChanged(object sender, EventArgs e)
        {
            textBox_MethyPosition_File.Enabled = checkBox_MethyPosition.Checked;
            button_SelectFile4.Enabled = checkBox_MethyPosition.Checked;
            checkBox_Level3.Enabled = checkBox_MethyPosition.Checked;
            if (!checkBox_Level3.Enabled)
                checkBox_Level3.Checked = false;
        }

        private void checkBox_Level3_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Level3.Enabled = checkBox_Level3.Checked;
            button_SelectFileLevel3.Enabled = checkBox_Level3.Checked;
        }

        private void checkBox_CodingGene_CheckedChanged(object sender, EventArgs e)
        {
            textBox_CodingGene_File.Enabled = checkBox_CodingGene.Checked;
            button_SelectFile5.Enabled = checkBox_CodingGene.Checked;
            radioButton1.Enabled = checkBox_CodingGene.Checked;
            radioButton2.Enabled = checkBox_CodingGene.Checked;
        }

        public Form1()
        {
            InitializeComponent();
            toolTip1.SetToolTip(textBox_Pseudogene_File,
                "假基因区域：有表头，以下三种格式任意均可：（可以有其他数据列，读取时会忽略）\n1、一列（Position），格式为“chr1:10000-12000”\n2、两列（chr、Position），格式为“chr1（分格）10000-12000”\n3、三列（chr、Start、End），格式为“chr1（分格）10000（分格）12000”");
            toolTip1.SetToolTip(textBox_Promoter_File, "启动子：无表头，两列，第1列为chr，第2列为启动子位点（End）（整数）");
            toolTip1.SetToolTip(textBox_Enhancer_File, "增强子：无表头，三列，第1列为chr，第2列为区域起始点（Start）（整数），第3列为区域结束点（End）（整数）");
            toolTip1.SetToolTip(textBox_Level3, "自动处理数据-5获得的level3的合并文件");


            toolTip1.SetToolTip(label1,
                "假基因区域：有表头，以下三种格式任意均可：（可以有其他数据列，读取时会忽略）\n1、一列（Position），格式为“chr1:10000-12000”\n2、两列（chr、Position），格式为“chr1（分格）10000-12000”\n3、三列（chr、Start、End），格式为“chr1（分格）10000（分格）12000”");
            toolTip1.SetToolTip(checkBox_Promoter, "启动子：无表头，两列，第1列为chr，第2列为启动子位点（End）（整数）");
            toolTip1.SetToolTip(checkBox_Enhancer, "增强子：无表头，三列，第1列为chr，第2列为区域起始点（Start）（整数），第3列为区域结束点（End）（整数）");
            toolTip1.SetToolTip(checkBox_Level3, "自动处理数据-5获得的level3的合并文件");
        }

        private void UpdateInfo(string text = "", bool clear = false, bool reset = false)
        {
            if (reset)
            {
                richTextBox_Info.Text = "等待开始";
                return;
            }

            if (clear)
            {
                richTextBox_Info.Text = "";
                return;
            }

            richTextBox_Info.Text += text;
            richTextBox_Info.Refresh();
            richTextBox_Info.Select(richTextBox_Info.TextLength, 0);
            richTextBox_Info.ScrollToCaret();
        }

        private void button_SelectFile1_Click(object sender, EventArgs e)
        {
            if (importFile1 == null)
            {
                importFile1 = new OpenFileDialog();
                importFile1.Multiselect = true;
                importFile1.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFile1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_Pseudogene_File.Text = importFile1.FileNames[0];
                if (importFile1.FileNames.Length > 1)
                    textBox_Pseudogene_File.Text += " 等" + importFile1.FileNames.Length + "个文件";
                selectFile1Changed = true;
            }
            DisposeData();
        }

        private void button_SelectFile2_Click(object sender, EventArgs e)
        {
            if (importFile2 == null)
            {
                importFile2 = new OpenFileDialog();
                importFile2.Multiselect = true;
                importFile2.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFile2.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_Promoter_File.Text = importFile2.FileNames[0];
                if (importFile2.FileNames.Length > 1)
                    textBox_Promoter_File.Text += " 等" + importFile2.FileNames.Length + "个文件";
                selectFile2Changed = true;
            }
            DisposeData();
        }

        private void button_SelectFile3_Click(object sender, EventArgs e)
        {
            if (importFile3 == null)
            {
                importFile3 = new OpenFileDialog();
                importFile3.Multiselect = true;
                importFile3.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFile3.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_Enhancer_File.Text = importFile3.FileNames[0];
                if (importFile3.FileNames.Length > 1)
                    textBox_Enhancer_File.Text += " 等" + importFile3.FileNames.Length + "个文件";
                selectFile3Changed = true;
            }
            DisposeData();
        }

        private void button_SelectFile4_Click(object sender, EventArgs e)
        {
            if (importFile4 == null)
            {
                importFile4 = new OpenFileDialog();
                importFile4.Multiselect = true;
                importFile4.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFile4.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_MethyPosition_File.Text = importFile4.FileNames[0];
                if (importFile4.FileNames.Length > 1)
                    textBox_MethyPosition_File.Text += " 等" + importFile4.FileNames.Length + "个文件";
                selectFile4Changed = true;
            }
            DisposeData();
        }

        private void button_SelectFileLevel3_Click(object sender, EventArgs e)
        {
            if (importFileLevel3 == null)
            {
                importFileLevel3 = new OpenFileDialog();
                importFileLevel3.Multiselect = true;
                importFileLevel3.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFileLevel3.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_Level3.Text = importFileLevel3.FileNames[0];
                if (importFileLevel3.FileNames.Length > 1)
                    textBox_Level3.Text += " 等" + importFileLevel3.FileNames.Length + "个文件";
                selectFile4Changed = true;
            }
            DisposeData();
        }

        private void button_SelectFile5_Click(object sender, EventArgs e)
        {
            if (importFile5 == null)
            {
                importFile5 = new OpenFileDialog();
                importFile5.Multiselect = true;
                importFile5.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = importFile5.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_CodingGene_File.Text = importFile5.FileNames[0];
                if (importFile5.FileNames.Length > 1)
                    textBox_CodingGene_File.Text += " 等" + importFile5.FileNames.Length + "个文件";
                selectFile5Changed = true;
            }
            DisposeData();
        }

        private void button_SelectOutputFile_Click(object sender, EventArgs e)
        {
            if (exportFile == null)
            {
                exportFile = new SaveFileDialog();
                exportFile.Filter = "txt制表符分隔（*.txt）|*.txt";
            }

            var result = exportFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_Output_File.Text = exportFile.FileName;
            }
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            //textBox1
            if (textBox_Pseudogene_File.Text == "")
            {
                MessageBox.Show("还没选择假基因区域文件！");
                return;
            }

            //textBox2
            if ((checkBox_Promoter.Checked) && (textBox_Promoter_File.Text == ""))
            {
                MessageBox.Show("还没选择启动子文件！");
                return;
            }

            //textBox3
            if ((checkBox_Enhancer.Checked) && (textBox_Enhancer_File.Text == ""))
            {
                MessageBox.Show("还没选择增强子文件！");
                return;
            }

            //启动增强都不选
            if ((!checkBox_Promoter.Checked) && (!checkBox_Enhancer.Checked))
            {
                MessageBox.Show("启动子增强子都不选，你想在哪找啊_(:3｣ ∠)_");
                return;
            }

            //textBox4
            if ((checkBox_MethyPosition.Checked) && (textBox_MethyPosition_File.Text == ""))
            {
                MessageBox.Show("还没选择甲基化位点文件！");
                return;
            }

            //textBox5
            if ((checkBox_CodingGene.Checked) && (textBox_CodingGene_File.Text == ""))
            {
                MessageBox.Show("还没选择编码基因文件！");
                return;
            }

            //甲基化位点和编码基因都不选
            /*if ((!checkBox_MethyPosition.Checked) && (!checkBox_CodingGene.Checked))
            {
                MessageBox.Show("甲基化位点和编码基因都不选，你究竟想找什么啊_(:3｣ ∠)_");
                return; ;
            }*/

            //textBox6
            if (textBox_Output_File.Text == "")
            {
                MessageBox.Show("还没选择输出文件！");
                return;
            }

            Stopwatch t = new Stopwatch();

            t.Reset();
            t.Start();

            UpdateInfo("", false, true);

            DisposeData();
            ReadData();
            bool result = StartWorking();

            t.Stop();
            if (result)
            {
                UpdateInfo("\n合并完成，共用时" + (t.ElapsedMilliseconds / 1000M).ToString("F1") + "s");
                MessageBox.Show("合并完成，共用时" + (t.ElapsedMilliseconds / 1000M).ToString("F1") + "s");
                selectFile1Changed = false;
                selectFile2Changed = false;
                selectFile3Changed = false;
                selectFile4Changed = false;
                selectFile5Changed = false;
            }
            else
            {
                UpdateInfo("\n遇到错误，程序终止");
            }
        }

        private void DisposeData()
        {
            if ((selectFile1Changed) && (pseudogenes != null))
                pseudogenes.Dispose();
            if (((selectFile2Changed) || (!checkBox_Promoter.Checked)) && (promoters != null))
                promoters.Dispose();
            if (((selectFile3Changed) || (!checkBox_Enhancer.Checked)) && (enhancers != null))
                enhancers.Dispose();
            if (((selectFile4Changed) || (!checkBox_MethyPosition.Checked)) && (methyExpPositions != null))
                methyExpPositions.Dispose();
            if (((selectFile5Changed) || (!checkBox_CodingGene.Checked)) && (methyExpPositions != null))
                methyExpPositions.Dispose();
        }

        private void ReadData()
        {
            if (selectFile1Changed)
            {
                file_Pseudogene = null;
                reader_Pseudogene = null;
                pseudogenes = new DataTable("Pseudogenes");
                pseudogenes.Columns.Add("Name", typeof (String));
                pseudogenes.Columns.Add("chr", typeof (String));
                pseudogenes.Columns.Add("Start", typeof (Int64));
                pseudogenes.Columns.Add("End", typeof (Int64));
                try
                {
                    string[] row_read = null, position = null;
                    string row = null, rowType = "Unknown";
                    int col_chr = -1, col_position = -1, col_start = -1, col_end = -1, col_name = -1;
                    int fileNo = 0, totalFile = importFile1.FileNames.Length;
                    bool isFirstRow, hasHeader;
                    foreach (string filePath in importFile1.FileNames)
                    {
                        fileNo++;
                        isFirstRow = true;
                        UpdateInfo("\n正在读入" + Path.GetFileName(filePath) + "，第" + fileNo + "个假基因文件，共" + totalFile + "个");
                        file_Pseudogene = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        reader_Pseudogene = new StreamReader(file_Pseudogene);
                        while (!reader_Pseudogene.EndOfStream)
                        {
                            row = reader_Pseudogene.ReadLine();
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                hasHeader = (row.ToLower().Contains("name")) ||
                                            (row.ToLower().Contains("position")) || (row.ToLower().Contains("start"));
                                if (hasHeader)
                                {
                                    row_read = row.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                                    for (int i = 0; i < row_read.Length; i++)
                                    {
                                        if (row_read[i].ToLower() == "chr")
                                            col_chr = i;
                                        else if (row_read[i].ToLower() == "position")
                                            col_position = i;
                                        else if (row_read[i].ToLower() == "start")
                                            col_start = i;
                                        else if (row_read[i].ToLower() == "end")
                                            col_end = i;
                                        else if ((row_read[i].ToLower().Contains("pseudo")) ||
                                                 (row_read[i].ToLower().Contains("name")))
                                            col_name = i;
                                    }
                                    if ((col_chr > -1) && (col_start > -1) && (col_end > -1))
                                        rowType = "Chr_Start_End";
                                    else if ((col_chr > -1) && (col_position > -1))
                                        rowType = "Chr_Position";
                                    else if ((col_chr == -1) && (col_position > -1))
                                        rowType = "Position";
                                    else
                                    {
                                        MessageBox.Show(
                                            "假基因文件" + Path.GetFileName(filePath) + "中表头无法识别，请确保文件格式符合提示中的格式！", "错误",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("假基因文件" + Path.GetFileName(filePath) + "中未找到表头！", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                continue; //读完表头，开始读数据
                                //.Split(new char[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
                            }
                            row_read = row.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                            string name = col_name == -1 ? "NONAME" : row_read[col_name];
                            switch (rowType)
                            {
                                case "Chr_Start_End":
                                    pseudogenes.Rows.Add(name, row_read[col_chr], Convert.ToInt64(row_read[col_start]),
                                        Convert.ToInt64(row_read[col_end]));
                                    break;
                                case "Chr_Position":
                                    position = row_read[col_position].Split('-');
                                    pseudogenes.Rows.Add(name, row_read[col_chr], Convert.ToInt64(position[0]),
                                        Convert.ToInt64(position[1]));
                                    break;
                                case "Position":
                                    position = row_read[col_position].Split(new char[] {':', '-'});
                                    pseudogenes.Rows.Add(name, position[0], Convert.ToInt64(position[1]),
                                        Convert.ToInt64(position[2]));
                                    break;
                            }
                        }
                        reader_Pseudogene.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (reader_Pseudogene != null)
                        reader_Pseudogene.Close();
                }
            }

            if (checkBox_Promoter.Checked)
            {
                if (selectFile2Changed)
                {
                    file_Promoter = null;
                    reader_Promoter = null;
                    promoters = new DataTable("Promoters");
                    promoters.Columns.Add("chr", typeof (String));
                    promoters.Columns.Add("End", typeof (Int64));
                    try
                    {
                        string[] row_read = null;
                        int fileNo = 0, totalFile = importFile2.FileNames.Length;
                        foreach (string filePath in importFile2.FileNames)
                        {
                            fileNo++;
                            UpdateInfo("\n正在读入" + Path.GetFileName(filePath) + "，第" + fileNo + "个启动子文件，共" + totalFile +
                                       "个");
                            file_Promoter = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            reader_Promoter = new StreamReader(file_Promoter);
                            while (!reader_Promoter.EndOfStream)
                            {
                                row_read = reader_Promoter.ReadLine()
                                    .Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                                if (row_read.Length < 2)
                                {
                                    MessageBox.Show("启动子列数少于2列", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                promoters.Rows.Add(row_read[0], Convert.ToInt64(row_read[1]));
                            }
                            reader_Promoter.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (reader_Promoter != null)
                            reader_Promoter.Close();
                    }
                }
            }

            if (checkBox_Enhancer.Checked)
            {
                if (selectFile3Changed)
                {
                    file_Enhancer = null;
                    reader_Enhancer = null;
                    enhancers = new DataTable("Enhancers");
                    enhancers.Columns.Add("chr", typeof (String));
                    enhancers.Columns.Add("Start", typeof (Int64));
                    enhancers.Columns.Add("End", typeof (Int64));
                    try
                    {
                        string[] row_read = null;
                        int fileNo = 0, totalFile = importFile3.FileNames.Length;
                        foreach (string filePath in importFile3.FileNames)
                        {
                            fileNo++;
                            UpdateInfo("\n正在读入" + Path.GetFileName(filePath) + "，第" + fileNo + "个增强子文件，共" + totalFile +
                                       "个");
                            file_Enhancer = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            reader_Enhancer = new StreamReader(file_Enhancer);
                            while (!reader_Enhancer.EndOfStream)
                            {
                                row_read = reader_Enhancer.ReadLine()
                                    .Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                                if (row_read.Length < 3)
                                {
                                    MessageBox.Show("增强子列数少于3列", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                enhancers.Rows.Add(row_read[0], Convert.ToInt64(row_read[1]),
                                    Convert.ToInt64(row_read[2]));
                            }
                            reader_Enhancer.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (reader_Enhancer != null)
                            reader_Enhancer.Close();
                    }
                }
            }

            if (checkBox_MethyPosition.Checked)
            {
                if (selectFile4Changed)
                {
                    file_MethyExpPosition = null;
                    reader_MethyExpPosition = null;
                    methyExpPositions = new DataTable("MethyExpPositions");
                    methyExpPositions.Columns.Add("cg", typeof (String));
                    methyExpPositions.Columns.Add("chr", typeof (String));
                    methyExpPositions.Columns.Add("Position", typeof (Int64));
                    try
                    {
                        string[] row_read = null;
                        int fileNo = 0, totalFile = importFile4.FileNames.Length;
                        foreach (string filePath in importFile4.FileNames)
                        {
                            fileNo++;
                            UpdateInfo("\n正在读入" + Path.GetFileName(filePath) + "，第" + fileNo + "个甲基化位点文件，共" +
                                       totalFile + "个");
                            file_MethyExpPosition = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            reader_MethyExpPosition = new StreamReader(file_MethyExpPosition);
                            while (!reader_MethyExpPosition.EndOfStream)
                            {
                                row_read = reader_MethyExpPosition.ReadLine()
                                    .Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                                if (row_read.Length < 3)
                                {
                                    MessageBox.Show("甲基化位点列数少于3列", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                methyExpPositions.Rows.Add(row_read[0], "chr" + row_read[1],
                                    Convert.ToInt64(row_read[2]));
                            }
                            reader_MethyExpPosition.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (reader_MethyExpPosition != null)
                            reader_MethyExpPosition.Close();
                    }
                }
            }

            if (checkBox_CodingGene.Checked)
            {
                if (selectFile5Changed)
                {
                    file_CodingGene = null;
                    reader_CodingGene = null;
                    codingGenes = new DataTable("CodingGenes");
                    codingGenes.Columns.Add("Name", typeof (String));
                    codingGenes.Columns.Add("chr", typeof (String));
                    codingGenes.Columns.Add("Start", typeof (Int64));
                    codingGenes.Columns.Add("End", typeof (Int64));
                    string[] row_raw = null, row_2ndColumn = null;
                    try
                    {
                        int fileNo = 0, totalFile = importFile5.FileNames.Length;
                        foreach (string filePath in importFile5.FileNames)
                        {
                            fileNo++;
                            UpdateInfo("\n正在读入" + Path.GetFileName(filePath) + "，第" + fileNo + "个编码基因文件，共" +
                                       totalFile + "个");
                            file_CodingGene = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            reader_CodingGene = new StreamReader(file_CodingGene);
                            while (!reader_CodingGene.EndOfStream)
                            {
                                row_raw = reader_CodingGene.ReadLine()
                                    .Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                                if (row_raw.Length < 2)
                                {
                                    MessageBox.Show("编码基因文件列格式错误！第一列应为名称，第二列格式应类似“chr1:10000000-10000030”", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                row_2ndColumn = row_raw[1].Split(new char[] {':', '-'},
                                    StringSplitOptions.RemoveEmptyEntries);
                                if (row_2ndColumn.Length < 3)
                                {
                                    MessageBox.Show("编码基因文件列格式错误！第一列应为名称，第二列格式应类似“chr1:10000000-10000030”", "错误",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                codingGenes.Rows.Add(row_raw[0], row_2ndColumn[0], Convert.ToInt64(row_2ndColumn[1]),
                                    Convert.ToInt64(row_2ndColumn[2]));
                            }
                            reader_CodingGene.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString() + "\n\n" + row_raw[0] + "\n" + row_raw[1], "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (reader_CodingGene != null)
                            reader_CodingGene.Close();
                    }
                }
            }
        }

        private bool StartWorking()
        {
            List<string> results1 = new List<string>(),
                results2 = new List<string>(),
                results3 = new List<string>(),
                results4 = new List<string>();
            int codingGenesAmount = 0;

            //表头

            #region 表头处理

            string s_Output = "假基因\tchr\t区域";
            if (checkBox_Promoter.Checked)
            {
                if (numericUpDown1.Value == 0)
                    s_Output += "\tTSS";
                else
                    s_Output += "\t启动子";
            }
            if (checkBox_Enhancer.Checked)
                s_Output += "\t增强子";
            if ((checkBox_CodingGene.Checked))
            {
                for (int i = 1; i <= codingGenesAmount; i++)
                    s_Output += "\t编码基因" + i.ToString() + "\t区域\t位置";
            }
            results1.Add(s_Output);
            results2.Add("chr\tPseudogene\t对应假基因区域\t对应启动子区域\tcg\t甲基化位点");
            results3.Add("chr\tPseudogene\t对应假基因区域\t对应增强子区域\tcg\t甲基化位点");
            results4.Add("chr\tPseudogene\t对应假基因区域\tcg\t甲基化位点");

            #endregion

            //开始处理

            #region 处理数据

            long n = 0, total = pseudogenes.Rows.Count;
            for (int chrNo = 1; chrNo <= 24; chrNo++)
            {
                string chr;
                switch (chrNo)
                {
                    case 23:
                        chr = "chrX";
                        break;
                    case 24:
                        chr = "chrY";
                        break;
                    default:
                        chr = "chr" + chrNo.ToString();
                        break;
                }
                foreach (DataRow pseudogene in pseudogenes.Select("chr='" + chr + "'"))
                {
                    #region 变量声明

                    n++;
                    Int64 currentPromoterStart = -1,
                        currentPromoterEnd = -1,
                        currentEnhancerStart = -1,
                        currentEnhancerEnd = -1;
                    ArrayList currentMethyExpPositions = null,
                        currentMethyExp_cg = null,
                        currentMethyExp_source = null,
                        currentMethyExp_sourcePosition = null,
                        currentMethyExp_itsPseudogene = null,
                        currentCodingGenesStart = null,
                        currentCodingGenesEnd = null,
                        extraInfo_Name = null,
                        extraInfo_Where = null;

                    Int64 pStart = (Int64) (pseudogene["Start"]), pEnd = (Int64) (pseudogene["End"]);
                    string pName = pseudogene["Name"].ToString();

                    #endregion

                    //启动子

                    #region 启动子

                    if (checkBox_Promoter.Checked)
                    {
                        UpdateInfo("\n正在搜索假基因对应的启动子，第" + n + "个，共" + total + "个...");
                        DataRow[] promotersOfTheSameChr = promoters.Select("chr='" + pseudogene["chr"] + "'",
                            "End asc");
                        Int64 min = Int64.MaxValue, difference = 0, solutionNo = -1;
                        for (int i = 0; i < promotersOfTheSameChr.Length; i++)
                        {
                            difference = pStart - (Int64) (promotersOfTheSameChr[i]["End"]);
                            if (difference <= 0)
                                break; //启动子尾在假基因头后面，说明之后的都不是了
                            else
                            {
                                if (min > difference)
                                {
                                    min = difference;
                                    solutionNo = i;
                                }
                            }
                        }
                        if ((min != Int64.MaxValue) && (solutionNo != -1))
                            //有解
                        {
                            currentPromoterEnd = (Int64) ((promotersOfTheSameChr[solutionNo])["End"]);
                            currentPromoterStart = currentPromoterEnd - Convert.ToInt64(numericUpDown1.Value);
                        }
                        UpdateInfo("完毕");
                    }

                    #endregion

                    //增强子

                    #region 增强子

                    if (checkBox_Enhancer.Checked)
                    {
                        UpdateInfo("\n正在搜索假基因对应的增强子，第" + n + "个，共" + total + "个...");
                        DataRow[] enhancersOfTheSameChr = enhancers.Select("chr='" + pseudogene["chr"] + "'",
                            "Start asc");
                        Int64 min = Int64.MaxValue, difference = 0, solutionNo = -1;
                        for (int i = 0; i < enhancersOfTheSameChr.Length; i++)
                        {
                            Int64 eStart = (Int64) (enhancersOfTheSameChr[i]["Start"]),
                                eEnd = (Int64) (enhancersOfTheSameChr[i]["End"]);
                            if ((eStart >= pStart) && (eStart <= pEnd))
                                //增强子头在假基因内，假基因与增强子相交或包含增强子，舍去
                                continue;
                            if ((eEnd >= pStart) && (eEnd <= pEnd))
                                //增强子尾在假基因内，假基因与增强子相交或包含增强子，舍去
                                continue;
                            if ((eStart <= pStart) && (eEnd >= pEnd))
                                //增强子包含假基因，舍去
                                continue;

                            if (eEnd < pStart)
                                //增强子在假基因前面
                            {
                                difference = pStart - eEnd;
                            }
                            else if (eStart > pEnd)
                                //增强子在假基因后面
                            {
                                difference = eStart - pEnd;
                            }

                            if (min > difference)
                            {
                                min = difference;
                                solutionNo = i;
                            }

                        }
                        if ((min != Int64.MaxValue) && (solutionNo != -1))
                            //有解
                        {
                            currentEnhancerStart = (Int64) ((enhancersOfTheSameChr[solutionNo])["Start"]);
                            currentEnhancerEnd = (Int64) ((enhancersOfTheSameChr[solutionNo])["End"]);
                        }
                        UpdateInfo("完毕");
                    }

                    #endregion

                    //甲基化位点

                    #region 甲基化位点

                    if (checkBox_MethyPosition.Checked)
                    {
                        UpdateInfo("\n正在搜索假基因对应的甲基化位点，第" + n + "个，共" + total + "个...");
                        DataRow[] methyExpPositionsOfTheSameChr =
                            methyExpPositions.Select("chr='" + pseudogene["chr"] + "'", "Position asc");
                        ArrayList solutionNo = new ArrayList(),
                            extraInfo1 = new ArrayList(),
                            extraInfo2 = new ArrayList(),
                            extraInfo3 = new ArrayList(),
                            extraInfo4 = new ArrayList();

                        Int64 position = -1;
                        for (int i = 0; i < methyExpPositionsOfTheSameChr.Length; i++)
                        {
                            position = (Int64) ((methyExpPositionsOfTheSameChr[i])["Position"]);
                            if ((position >= currentPromoterStart) && (position <= currentPromoterEnd))
                            {
                                solutionNo.Add(position);
                                extraInfo1.Add((methyExpPositionsOfTheSameChr[i])["cg"].ToString());
                                extraInfo2.Add("启动子内");
                                extraInfo3.Add(currentPromoterStart + "-" + currentPromoterEnd);
                                extraInfo4.Add(pName + "\t" + pStart + "-" + pEnd);
                            }
                            else if ((position >= currentEnhancerStart) && (position <= currentEnhancerEnd))
                            {
                                solutionNo.Add(position);
                                extraInfo1.Add((methyExpPositionsOfTheSameChr[i])["cg"].ToString());
                                extraInfo2.Add("增强子内");
                                extraInfo3.Add(currentEnhancerStart + "-" + currentEnhancerEnd);
                                extraInfo4.Add(pName + "\t" + pStart + "-" + pEnd);
                            }
                            else if ((position >= pStart) && (position <= pEnd))
                            {
                                solutionNo.Add(position);
                                extraInfo1.Add((methyExpPositionsOfTheSameChr[i])["cg"].ToString());
                                extraInfo2.Add("假基因内");
                                extraInfo3.Add("");
                                extraInfo4.Add(pName + "\t" + pStart + "-" + pEnd);
                            }
                        }
                        if (solutionNo.Count > 0)
                            //有解
                        {
                            currentMethyExpPositions = solutionNo;
                            currentMethyExp_cg = extraInfo1;
                            currentMethyExp_source = extraInfo2;
                            currentMethyExp_sourcePosition = extraInfo3;
                            currentMethyExp_itsPseudogene = extraInfo4;
                        }
                        UpdateInfo("完毕");
                    }

                    #endregion

                    //编码基因

                    #region 编码基因

                    if (checkBox_CodingGene.Checked)
                    {
                        UpdateInfo("\n正在搜索假基因对应的编码基因，第" + n + "个，共" + total + "个...");
                        DataRow[] codingGenesOfTheSameChr = codingGenes.Select("chr='" + pseudogene["chr"] + "'",
                            "Start asc");
                        currentCodingGenesStart = new ArrayList();
                        currentCodingGenesEnd = new ArrayList();
                        extraInfo_Name = new ArrayList();
                        extraInfo_Where = new ArrayList();

                        if (radioButton1.Checked)
                        {
                            #region 启动子处

                            if (checkBox_Promoter.Checked)
                            {
                                Int64 difference = 0, solutionNo = -1, min = Int64.MaxValue;
                                UpdateInfo("\n正在搜索第" + n + "个假基因对应的启动子处的编码基因，共" + total + "个假基因");
                                for (int i = 0; i < codingGenesOfTheSameChr.Length; i++)
                                {
                                    Int64 cgStart = (Int64) (codingGenesOfTheSameChr[i]["Start"]),
                                        cgEnd = (Int64) (codingGenesOfTheSameChr[i]["End"]);
                                    if (((cgStart >= currentPromoterStart) && (cgStart <= currentPromoterEnd)) ||
                                        ((cgEnd >= currentPromoterStart) && (cgEnd <= currentPromoterEnd)) ||
                                        ((cgStart <= currentPromoterStart) && (cgEnd >= currentPromoterEnd)))
                                        //编码基因头或尾在启动子内，启动子与编码基因相交或包含编码基因；或者编码基因包含启动子
                                    {
                                        currentCodingGenesStart.Add(cgStart);
                                        currentCodingGenesEnd.Add(cgEnd);
                                        extraInfo_Name.Add(codingGenesOfTheSameChr[i]["Name"]);
                                        if (((cgStart >= currentPromoterStart) && (cgStart <= currentPromoterEnd)) &&
                                            ((cgEnd >= currentPromoterStart) && (cgEnd <= currentPromoterEnd)))
                                            extraInfo_Where.Add("启动子内包含");
                                        else if (((cgStart <= currentPromoterStart) && (cgEnd >= currentPromoterEnd)))
                                            extraInfo_Where.Add("包含了启动子");
                                        else
                                            extraInfo_Where.Add("与启动子相交");
                                        continue;
                                    }

                                    if (currentCodingGenesStart.Count > 0)
                                        //如果已经有包含或相交的，就不考虑附近的了
                                        continue;
                                    else
                                    //如果没有包含或相交，则考虑附近的
                                    {
                                        if (cgEnd < currentPromoterStart)
                                            //编码基因在启动子前面
                                        {
                                            difference = currentPromoterStart - cgEnd;
                                        }
                                        else if (cgStart > currentPromoterEnd)
                                            //编码基因在启动子后面
                                        {
                                            difference = cgStart - currentPromoterEnd;
                                        }

                                        if (min > difference)
                                        {
                                            min = difference;
                                            solutionNo = i;
                                        }
                                    }
                                }
                                if (currentCodingGenesStart.Count == 0)
                                    //没有包含或相交，只有附近的或没有
                                {
                                    if (solutionNo != -1)
                                        //有附近的
                                    {
                                        currentCodingGenesStart.Add(
                                            (Int64) (codingGenesOfTheSameChr[solutionNo]["Start"]));
                                        currentCodingGenesEnd.Add(
                                            (Int64) (codingGenesOfTheSameChr[solutionNo]["End"]));
                                        extraInfo_Name.Add(codingGenesOfTheSameChr[solutionNo]["Name"]);
                                        extraInfo_Where.Add("启动子附近");
                                    }
                                    else
                                    //附近的都没有
                                    {
                                        currentCodingGenesStart.Add(-1);
                                        currentCodingGenesEnd.Add(-1);
                                        extraInfo_Name.Add("启动子处无编码基因");
                                        extraInfo_Where.Add("N/A");
                                    }
                                }
                            }

                            #endregion

                            #region 增强子处

                            int count0 = currentCodingGenesStart.Count;
                            if (checkBox_Enhancer.Checked)
                            {
                                Int64 difference = 0, solutionNo = -1, min = Int64.MaxValue;
                                UpdateInfo("\n正在搜索第" + n + "个假基因对应的增强子处的编码基因，共" + total + "个假基因");
                                for (int i = 0; i < codingGenesOfTheSameChr.Length; i++)
                                {
                                    Int64 cgStart = (Int64) (codingGenesOfTheSameChr[i]["Start"]),
                                        cgEnd = (Int64) (codingGenesOfTheSameChr[i]["End"]);
                                    if (((cgStart >= currentEnhancerStart) && (cgStart <= currentEnhancerEnd)) ||
                                        ((cgEnd >= currentEnhancerStart) && (cgEnd <= currentEnhancerEnd)) ||
                                        ((cgStart <= currentEnhancerStart) && (cgEnd >= currentEnhancerEnd)))
                                        //编码基因头或尾在增强子内，增强子与编码基因相交或包含编码基因；或者编码基因包含增强子
                                    {
                                        currentCodingGenesStart.Add(cgStart);
                                        currentCodingGenesEnd.Add(cgEnd);
                                        extraInfo_Name.Add(codingGenesOfTheSameChr[i]["Name"]);
                                        if (((cgStart >= currentEnhancerStart) && (cgStart <= currentEnhancerEnd)) &&
                                            ((cgEnd >= currentEnhancerStart) && (cgEnd <= currentEnhancerEnd)))
                                            extraInfo_Where.Add("增强子内包含");
                                        else if ((cgStart <= currentEnhancerStart) && (cgEnd >= currentEnhancerEnd))
                                            extraInfo_Where.Add("包含了增强子");
                                        else
                                            extraInfo_Where.Add("与增强子相交");
                                        continue;
                                    }

                                    if (currentCodingGenesStart.Count > count0)
                                        //如果已经有包含或相交的，就不考虑附近的了
                                        continue;
                                    else
                                    //如果没有包含或相交，则考虑附近的
                                    {
                                        if (cgEnd < currentEnhancerStart)
                                            //编码基因在增强子前面
                                        {
                                            difference = currentEnhancerStart - cgEnd;
                                        }
                                        else if (cgStart > currentEnhancerEnd)
                                            //编码基因在增强子后面
                                        {
                                            difference = cgStart - currentEnhancerEnd;
                                        }

                                        if (min > difference)
                                        {
                                            min = difference;
                                            solutionNo = i;
                                        }
                                    }
                                }
                                if (currentCodingGenesStart.Count == count0)
                                    //没有包含或相交，只有附近的或没有
                                {
                                    if (solutionNo != -1)
                                        //有附近的
                                    {
                                        currentCodingGenesStart.Add(
                                            (Int64) (codingGenesOfTheSameChr[solutionNo]["Start"]));
                                        currentCodingGenesEnd.Add(
                                            (Int64) (codingGenesOfTheSameChr[solutionNo]["End"]));
                                        extraInfo_Name.Add(codingGenesOfTheSameChr[solutionNo]["Name"]);
                                        extraInfo_Where.Add("增强子附近");
                                    }
                                    else
                                    //附近的都没有
                                    {
                                        currentCodingGenesStart.Add(-1);
                                        currentCodingGenesEnd.Add(-1);
                                        extraInfo_Name.Add("增强子处无编码基因");
                                        extraInfo_Where.Add("N/A");
                                    }
                                }
                            }

                            #endregion
                        } //radioButton1
                        else if (radioButton2.Checked)
                        {
                            #region 假基因前后

                            Int64 difference_Former = 0,
                                difference_Latter = 0,
                                solutionNo_Former = -1,
                                solutionNo_Latter = -1,
                                min_Former = Int64.MaxValue,
                                min_Latter = Int64.MaxValue;
                            UpdateInfo("\n正在搜索第" + n + "个假基因前后的编码基因，共" + total + "个假基因");
                            for (int i = 0; i < codingGenesOfTheSameChr.Length; i++)
                            {
                                Int64 cgStart = (Int64) (codingGenesOfTheSameChr[i]["Start"]),
                                    cgEnd = (Int64) (codingGenesOfTheSameChr[i]["End"]);
                                if (((cgStart >= pStart) && (cgStart <= pEnd)) ||
                                    ((cgEnd >= pStart) && (cgEnd <= pEnd)) ||
                                    ((cgStart <= pStart) && (cgEnd >= pEnd)))
                                    //编码基因头或尾在假基因内，假基因与编码基因相交或包含编码基因；或者编码基因包含假基因：都不要
                                {
                                    //currentCodingGenesStart.Add(cgStart);
                                    //currentCodingGenesEnd.Add(cgEnd);
                                    //extraInfo_Name.Add(codingGenesOfTheSameChr[i]["Name"]);
                                    //if (((cgStart >= pStart) && (cgStart <= pEnd)) &&
                                    //    ((cgEnd >= pStart) && (cgEnd <= pEnd)))
                                    //    extraInfo_Where.Add("假基因内包含");
                                    //else if ((cgStart <= pStart) && (cgEnd >= pEnd))
                                    //    extraInfo_Where.Add("包含了假基因");
                                    //else
                                    //    extraInfo_Where.Add("与假基因相交");
                                    continue;
                                }

                                //考虑前后附近的
                                {
                                    if (cgEnd < pStart)
                                        //编码基因在假基因前面
                                    {
                                        difference_Former = pStart - cgEnd;
                                        if (min_Former > difference_Former)
                                        {
                                            min_Former = difference_Former;
                                            solutionNo_Former = i;
                                        }
                                    }
                                    else if (cgStart > pEnd)
                                        //编码基因在假基因后面
                                    {
                                        difference_Latter = cgStart - pEnd;
                                        if (min_Latter > difference_Latter)
                                        {
                                            min_Latter = difference_Latter;
                                            solutionNo_Latter = i;
                                        }
                                    }
                                }
                            }

                            if (solutionNo_Former != -1)
                                //有前面的
                            {
                                currentCodingGenesStart.Add(
                                    (Int64) (codingGenesOfTheSameChr[solutionNo_Former]["Start"]));
                                currentCodingGenesEnd.Add(
                                    (Int64) (codingGenesOfTheSameChr[solutionNo_Former]["End"]));
                                extraInfo_Name.Add(codingGenesOfTheSameChr[solutionNo_Former]["Name"]);
                                extraInfo_Where.Add("假基因前方");
                            }
                            else
                            //没有前面
                            {
                                currentCodingGenesStart.Add(-1);
                                currentCodingGenesEnd.Add(-1);
                                extraInfo_Name.Add("假基因前方无编码基因");
                                extraInfo_Where.Add("N/A");
                            }

                            if (solutionNo_Latter != -1)
                                //有后面的
                            {
                                currentCodingGenesStart.Add(
                                    (Int64) (codingGenesOfTheSameChr[solutionNo_Latter]["Start"]));
                                currentCodingGenesEnd.Add(
                                    (Int64) (codingGenesOfTheSameChr[solutionNo_Latter]["End"]));
                                extraInfo_Name.Add(codingGenesOfTheSameChr[solutionNo_Latter]["Name"]);
                                extraInfo_Where.Add("假基因后方");
                            }
                            else
                            //没有后面
                            {
                                currentCodingGenesStart.Add(-1);
                                currentCodingGenesEnd.Add(-1);
                                extraInfo_Name.Add("假基因后方无编码基因");
                                extraInfo_Where.Add("N/A");
                            }

                            #endregion
                        }
                        UpdateInfo("完毕");
                    }

                    #endregion

                    //写输出列表

                    #region 写输出列表

                    string row_Output = pseudogene["Name"].ToString() + "\t" + pseudogene["chr"].ToString() + "\t" +
                                        pseudogene["Start"].ToString() + "-" + pseudogene["End"].ToString();
                    if (checkBox_Promoter.Checked)
                    {
                        if (numericUpDown1.Value == 0)
                            row_Output += "\t" + currentPromoterStart.ToString();
                        else
                            row_Output += "\t" + currentPromoterStart.ToString() + "-" + currentPromoterEnd;
                    }
                    if (checkBox_Enhancer.Checked)
                        row_Output += "\t" + currentEnhancerStart.ToString() + "-" + currentEnhancerEnd;
                    if (checkBox_CodingGene.Checked)
                    {
                        if (currentCodingGenesStart != null)
                        {
                            int cgN = 0;
                            for (int i = 0; i < currentCodingGenesStart.Count; i++)
                            {
                                row_Output += "\t" + extraInfo_Name[i].ToString();
                                row_Output += "\t" + currentCodingGenesStart[i].ToString() + "-" +
                                              currentCodingGenesEnd[i].ToString();
                                row_Output += "\t" + extraInfo_Where[i].ToString();
                                cgN++;
                            }
                            if (cgN > codingGenesAmount)
                                codingGenesAmount = cgN;
                        }
                    }
                    results1.Add(row_Output);
                    //writer_Output1.WriteLine(row_Output);

                    //输出甲基化位点信息
                    if (checkBox_MethyPosition.Checked)
                    {
                        if (currentMethyExpPositions != null)
                        {
                            string row_Output_1 = null, row_Output_2 = null;
                            for (int i = 0; i < currentMethyExpPositions.Count; i++)
                            {
                                row_Output_1 = pseudogene["chr"] + "\t" +
                                               currentMethyExp_itsPseudogene[i] + "\t" +
                                               currentMethyExp_sourcePosition[i] + "\t" +
                                               currentMethyExp_cg[i] + "\t" +
                                               currentMethyExpPositions[i]; //启动子、增强子中
                                row_Output_2 = pseudogene["chr"] + "\t" +
                                               currentMethyExp_itsPseudogene[i] + "\t" +
                                               currentMethyExp_cg[i] + "\t" +
                                               currentMethyExpPositions[i];
                                //假基因中
                                if (currentMethyExp_source[i].ToString() == "启动子内")
                                    results2.Add(row_Output_1);
                                else if (currentMethyExp_source[i].ToString() == "增强子内")
                                    results3.Add(row_Output_1);
                                else if (currentMethyExp_source[i].ToString() == "假基因内")
                                    results4.Add(row_Output_2);
                            }
                        }
                    }

                    #endregion
                }
            }

            #endregion

            #region 查找level3值
            if (checkBox_Level3.Checked)
            {
                //寻找level3值
                UpdateInfo("\n正在查找level3值...");
                StreamReader reader = new StreamReader(importFileLevel3.FileName);
                string row = reader.ReadLine();
                string[] row_split;
                results2[0] += "\t" + row;
                results3[0] += "\t" + row;
                results4[0] += "\t" + row;

                //做cg的hashtable
                Hashtable ht_MethyPosition = new Hashtable(); // key: cg; value: 整行
                for (int i = 1; i < results2.Count; i++)
                {
                    row_split = results2[i].Split(splitter1);
                    if (!ht_MethyPosition.Contains(row_split[4]))
                        ht_MethyPosition.Add(row_split[4], "");
                }
                for (int i = 1; i < results3.Count; i++)
                {
                    row_split = results3[i].Split(splitter1);
                    if (!ht_MethyPosition.Contains(row_split[4]))
                        ht_MethyPosition.Add(row_split[4], "");
                }
                for (int i = 1; i < results4.Count; i++)
                {
                    row_split = results4[i].Split(splitter1);
                    if (!ht_MethyPosition.Contains(row_split[3]))
                        ht_MethyPosition.Add(row_split[3], "");
                }

                // 读取level3，记录cg在hashtable里有的行
                while (!reader.EndOfStream)
                {
                    row = reader.ReadLine();
                    row_split = row.Split(splitter1);
                    if (ht_MethyPosition.Contains(row_split[0]))
                        ht_MethyPosition[row_split[0]] = row;
                }
                reader.Close();

                //将level3信息附在结果后方
                for (int i = 1; i < results2.Count; i++)
                {
                    row_split = results2[i].Split(splitter1);
                    results2[i] += "\t" + ht_MethyPosition[row_split[4]];
                }
                for (int i = 1; i < results3.Count; i++)
                {
                    row_split = results3[i].Split(splitter1);
                    results3[i] += "\t" + ht_MethyPosition[row_split[4]];
                }
                for (int i = 1; i < results4.Count; i++)
                {
                    row_split = results4[i].Split(splitter1);
                    results4[i] += "\t" + ht_MethyPosition[row_split[3]];
                }
                UpdateInfo("完毕");
            }
            #endregion

            #region 文件操作，输出
            FileStream stream = null;
            StreamWriter writer = null;

            UpdateInfo("\n正在输出结果...");
            if (checkBox_Promoter.Checked)
                stream =
                    new FileStream(
                        exportFile.FileName.Replace(Path.GetExtension(textBox_Output_File.Text),
                            "-启动子长度" + numericUpDown1.Value.ToString("F0") + Path.GetExtension(textBox_Output_File.Text)),
                        FileMode.Create, FileAccess.Write);
            else
                stream = new FileStream(exportFile.FileName, FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(stream, Encoding.Unicode);
            foreach (string t in results1)
            {
                writer.WriteLine(t);
            }
            writer.Close();

            if (checkBox_MethyPosition.Checked)
            {
                stream =
                    new FileStream(
                        exportFile.FileName.Replace(".txt",
                            "-启动子长度" + numericUpDown1.Value.ToString("F0") + "-甲基化位点-启动子中.txt"), FileMode.Create,
                        FileAccess.Write);
                writer = new StreamWriter(stream, Encoding.Unicode);
                foreach (string t in results2)
                {
                    writer.WriteLine(t);
                }
                writer.Close();

                stream =
                    new FileStream(
                        exportFile.FileName.Replace(".txt",
                            "-启动子长度" + numericUpDown1.Value.ToString("F0") + "-甲基化位点-增强子中.txt"), FileMode.Create,
                        FileAccess.Write);
                writer = new StreamWriter(stream, Encoding.Unicode);
                foreach (string t in results3)
                {
                    writer.WriteLine(t);
                }
                writer.Close();

                stream =
                    new FileStream(
                        exportFile.FileName.Replace(".txt",
                            "-启动子长度" + numericUpDown1.Value.ToString("F0") + "-甲基化位点-假基因中.txt"), FileMode.Create,
                        FileAccess.Write);
                writer = new StreamWriter(stream, Encoding.Unicode);
                foreach (string t in results4)
                {
                    writer.WriteLine(t);
                }
                writer.Close();
            }
            UpdateInfo("完毕");
            #endregion

            return true;

        }
    }
}
