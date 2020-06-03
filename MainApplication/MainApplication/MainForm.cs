using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace MainApplication
{
    public partial class MainForm : Form
    {
        //we will store in here the original width of column
        Dictionary<string, int> columnsOriginalWidth = new Dictionary<string, int>();
        String fileName = Application.StartupPath + "/grid state.xml";

        public MainForm()
        {
            InitializeComponent();
        }

        //now lets serialize the grid upon clicking the button
        private void btnSaveGridState_Click(object sender, EventArgs e)
        {
            //serialize a List of GridColumn since it has more than one column
            XmlSerializer serializer = new XmlSerializer(typeof(List<GridColumn>));

            //lets write the file on the exe's location
            TextWriter writer = new StreamWriter(fileName);

            List<GridColumn> xmlColumnCollection = new List<GridColumn>();
            bool HasError = false;

            try
            {
                //loop on all grid columns
                foreach (DataGridViewColumn gridColumn in dataGridView1.Columns)
                {
                    //create a new instance of gridcolumn and store the current state of column
                    GridColumn xmlColumn = new GridColumn();

                    xmlColumn.Name = gridColumn.Name;
                    xmlColumn.Index = gridColumn.DisplayIndex;
                    xmlColumn.Width = gridColumn.Width;

                    //store the xmlColumn
                    xmlColumnCollection.Add(xmlColumn);
                }

                //now lets serialize
                try
                {
                    serializer.Serialize(writer, xmlColumnCollection);
                }
                catch (Exception ex)
                {
                    //lets check if it has error
                    MessageBox.Show("Error: " + ex.Message);
                    HasError = true;
                }
            }
            finally
            {
                //make sure to close the stream
                writer.Close();

                //show this if successful
                if (!HasError)
                    MessageBox.Show("Done!");
            }
        }

        //lets reload the saved state and apply it on grid
        private void MainForm_Load(object sender, EventArgs e)
        {
            //lets store the original width before we apply the saved state
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                columnsOriginalWidth.Add(column.Name, column.Width);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<GridColumn>));
            TextReader reader = null;
            List<GridColumn> xmlColumnCollection = new List<GridColumn>();

            //lets check if file is existing first
            if (File.Exists(fileName))
            {
                try
                {
                    reader = new StreamReader(fileName);

                    //deserialize the file
                    xmlColumnCollection = serializer.Deserialize(reader) as List<GridColumn>;

                    //lets now apply the settings on the grid
                    foreach (GridColumn xmlColumn in xmlColumnCollection)
                    {
                        dataGridView1.Columns[xmlColumn.Name].DisplayIndex = xmlColumn.Index;
                        dataGridView1.Columns[xmlColumn.Name].Width = xmlColumn.Width;
                    }
                }
                finally
                {
                    //make sure to close the stream
                    reader.Close();
                }
            }
        }

        //lets reset everything
        private void btnResetGridState_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.DisplayIndex = column.Index;
                column.Width = columnsOriginalWidth[column.Name];
            }

            //delete the xml file if existing
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
    //now lets test again

    //first we will create a class which will represent our DataGridViewColumn
    public class GridColumn
    {
        //this will store the name of the column
        [XmlElement]
        public String Name { get; set; }

        //stores the index of the column
        [XmlElement]
        public int Index { get; set; }

        //stores the width of the column
        [XmlElement]
        public int Width { get; set; }
    }
}
