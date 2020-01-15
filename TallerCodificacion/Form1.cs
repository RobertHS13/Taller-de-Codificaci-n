using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TallerCodificacion
{
    public partial class Form1 : Form
    {
        DataTable simbolos = new DataTable();
        int pos, l, ns, nb, bpc, tb;

        private void botonExaminar_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog()
            {
                FileName = "Selecciona un archivo de texto",
                Filter = "Text files (*.txt) | *.txt",
                Title = "Seleccione archivo"
            };

            if(fd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var ruta = fd.FileName;
                    txtExaminar.Text = ruta;
                    rtxtTexto.Text = File.ReadAllText(ruta);
                }
                catch(SecurityException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                procesarTexto();
            }
        }

        public Form1()
        {
            InitializeComponent();
            simbolos.Columns.Add("Ind", typeof(int));
            simbolos.Columns.Add("Simbolo", typeof(string));
            simbolos.Columns.Add("Frec", typeof(int));
            simbolos.Columns.Add("Visitado", typeof(bool));
            simbolos.Columns.Add("Izq", typeof(int));
            simbolos.Columns.Add("Der", typeof(int));
            simbolos.Columns.Add("Código", typeof(string));
            simbolos.Columns.Add("Bits", typeof(int));
            simbolos.Columns.Add("Bits por caracter", typeof(int));

            dgv.DataSource = simbolos;
            dgv.Columns["Visitado"].Visible = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void procesarTexto()
        {
            string s, s1, s2;
            int j, f1, f2, p1, p2;

            l = rtxtTexto.TextLength;
            j = -1;

            //Ingresa los primeros nodos
            for (int i = 0; i < l; i++)
            {
                s = rtxtTexto.Text.Substring(i, 1);
                pos = buscarSimbolo(s);
                if (pos >= 0)
                    simbolos.Rows[pos][2] = Int32.Parse(simbolos.Rows[pos][2].ToString());
                else
                    simbolos.Rows.Add(new Object[] { j += 1, s, 1, false, -1, -1, "" });
            }
            ns = j;

            //Generar árbol

            for (int i = 0; i < ns; i++)
            {
                p1 = buscarMenor();
                f1 = Int32.Parse(simbolos.Rows[p1][2].ToString());
                s1 = simbolos.Rows[p1][1].ToString();
                simbolos.Rows[p1]["Visitado"] = true;

                p2 = buscarMenor();
                f2 = Int32.Parse(simbolos.Rows[p2][2].ToString());
                s2 = simbolos.Rows[p2][1].ToString();
                simbolos.Rows[p2]["Visitado"] = true;

                simbolos.Rows.Add(new Object[] { j += 1, s1 + s2, f1 + f2, false, p1, p2, "" });
            }

            //Generar Código

            for (int i = 1; i <= ns; i++)
            {
                asignarCero(Int32.Parse(simbolos.Rows[simbolos.Rows.Count - i]["Izq"].ToString()));
                asignarUno(Int32.Parse(simbolos.Rows[simbolos.Rows.Count - i]["Der"].ToString()));
            }

            //Calcular Longitudes


        }

        private void asignarCero(int p)
        {
            if (Int32.Parse(simbolos.Rows[p]["Izq"].ToString()) == -1)
                simbolos.Rows[p]["Código"] = simbolos.Rows[p]["Código"].ToString() + "0";
            else
            {
                asignarCero(Int32.Parse(simbolos.Rows[p]["Izq"].ToString()));
                asignarCero(Int32.Parse(simbolos.Rows[p]["Der"].ToString()));
            }
        }

        private void asignarUno(int p)
        {
            if (Int32.Parse(simbolos.Rows[p]["Der"].ToString()) == -1)
                simbolos.Rows[p]["Código"] = simbolos.Rows[p]["Código"].ToString() + "1";
            else
            {
                asignarUno(Int32.Parse(simbolos.Rows[p]["Izq"].ToString()));
                asignarUno(Int32.Parse(simbolos.Rows[p]["Der"].ToString()));
            }
        }

        private int buscarMenor()
        {
            int m, f, pos = 0;
            bool b;
            m = l + 1;
            for (int i = 0; i < simbolos.Rows.Count; i++)
            {
                f = Int32.Parse(simbolos.Rows[i][2].ToString());
                b = Boolean.Parse(simbolos.Rows[i]["Visitado"].ToString());
                if(f<m && !b)
                {
                    m = f;
                    pos = i;
                }
            }
            return pos;
        }

        private int buscarSimbolo(string s)
        {
            int p = -1;
            if(simbolos.Rows.Count > 1)
                for (int i = 0; i < simbolos.Rows.Count; i++)
                    if (s == simbolos.Rows[i][1].ToString())
                        return i;
            return p;
        }
    }
}
