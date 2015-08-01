Imports System.Net
Imports System.IO

'
' .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
'| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
'| |   ______     | || |     _____    | || |  _______     | || |  ___  ____   | || |  ____  ____  | || |     ____     | || |  _________   | || |  _________   | |
'| |  |_   _ \    | || |    |_   _|   | || | |_   __ \    | || | |_  ||_  _|  | || | |_   ||   _| | || |   .'    '.   | || | |_   ___  |  | || | |_   ___  |  | |
'| |    | |_) |   | || |      | |     | || |   | |__) |   | || |   | |_/ /    | || |   | |__| |   | || |  |  .--.  |  | || |   | |_  \_|  | || |   | |_  \_|  | |
'| |    |  __'.   | || |      | |     | || |   |  __ /    | || |   |  __'.    | || |   |  __  |   | || |  | |    | |  | || |   |  _|      | || |   |  _|      | |
'| |   _| |__) |  | || |     _| |_    | || |  _| |  \ \_  | || |  _| |  \ \_  | || |  _| |  | |_  | || |  |  `--'  |  | || |  _| |_       | || |  _| |_       | |
'| |  |_______/   | || |    |_____|   | || | |____| |___| | || | |____||____| | || | |____||____| | || |   '.____.'   | || | |_____|      | || | |_____|      | |
'| |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | |
'| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
''----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'



Public Class Form1

    Private Sub listarFTP(ByVal URL As String, ByVal bk As String, ByVal pw As String)
        Dim peticion As FtpWebRequest = Nothing
        Dim respuesta As FtpWebResponse = Nothing
        Dim reader As StreamReader = Nothing
        Try
            peticion = CType(WebRequest.Create(URL), FtpWebRequest)
            peticion.Credentials = New NetworkCredential(bk, pw)
            peticion.Method = WebRequestMethods.Ftp.ListDirectory
            respuesta = CType(peticion.GetResponse(), FtpWebResponse)
            reader = New StreamReader(respuesta.GetResponseStream())
            While (reader.Peek() > -1)
                ListBox1.Items.Add(reader.ReadLine())

            End While
            ToolStripStatusLabel1.Text = "Conexion exitosa!"
        Catch ex As UriFormatException
            ToolStripStatusLabel2.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If reader IsNot Nothing Then reader.Close()

        End Try
    End Sub


    Private Sub descargarFTP(ByVal URL As String, ByVal bk As String, ByVal pw As String)
        Dim peticion As FtpWebRequest = Nothing
        Dim respuesta As FtpWebResponse = Nothing
        Dim respStrm As Stream = Nothing
        Dim fileStrm As FileStream = Nothing
        Try
            peticion = CType(WebRequest.Create(URL), FtpWebRequest)
            peticion.Credentials = New NetworkCredential(bk, pw)
            peticion.Method = WebRequestMethods.Ftp.DownloadFile
            respuesta = CType(peticion.GetResponse(), FtpWebResponse)
            respStrm = respuesta.GetResponseStream()
            SaveFileDialog1.FileName = Path.GetFileName(peticion.RequestUri.LocalPath())
            If (SaveFileDialog1.ShowDialog() = DialogResult.OK) Then
                fileStrm = File.Create(SaveFileDialog1.FileName)
                Dim buff(1024) As Byte
                Dim bytesRead As Integer = 0
                While (True)
                    bytesRead = respStrm.Read(buff, 0, buff.Length)
                    If (bytesRead = 0) Then Exit While
                    fileStrm.Write(buff, 0, bytesRead)

                End While
                ToolStripStatusLabel1.Text = "Descarga correcta!"
            End If

        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Catch ex As IOException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If respStrm IsNot Nothing Then respStrm.Close()
            If fileStrm IsNot Nothing Then fileStrm.Close()

        End Try
    End Sub

    Private Sub eliminarFTP(ByVal URL As String, ByVal bk As String, ByVal pw As String)
        Dim peticion As FtpWebRequest = Nothing
        Dim respuesta As FtpWebResponse = Nothing
        Try
            peticion = CType(WebRequest.Create(URL), FtpWebRequest)
            peticion.Credentials = New NetworkCredential(bk, pw)
            peticion.Method = WebRequestMethods.Ftp.DeleteFile
            respuesta = CType(peticion.GetResponse(), FtpWebResponse)
            ToolStripStatusLabel1.Text = "Archivo eliminado!"
        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If respuesta IsNot Nothing Then respuesta.Close()
        End Try

    End Sub

    Private Sub subirFTP(ByVal filename As String, ByVal URL As String, ByVal bk As String, ByVal pw As String)
        Dim peticion As FtpWebRequest = Nothing
        Dim respuesta As FtpWebResponse = Nothing
        Dim petiStrm As Stream = Nothing
        Dim fileStrm As FileStream = Nothing
        Try
            peticion = CType(WebRequest.Create(URL), FtpWebRequest)
            peticion.Credentials = New NetworkCredential(bk, pw)
            peticion.Method = WebRequestMethods.Ftp.UploadFile
            peticion.Timeout = System.Threading.Timeout.Infinite
            peticion.Proxy = Nothing
            petiStrm = peticion.GetRequestStream()
            Dim buff(2048) As Byte
            Dim bytesRead As Integer = 0
            fileStrm = File.OpenRead(filename)
            Do While (True)
                bytesRead = fileStrm.Read(buff, 0, buff.Length)
                If (bytesRead = 0) Then Exit Do
                petiStrm.Write(buff, 0, bytesRead)
            Loop
            petiStrm.Close()
            respuesta = CType(peticion.GetResponse, FtpWebResponse)
            ToolStripStatusLabel1.Text = "Archivo subido satisfactoriamente!"
        Catch ex As UriFormatException
            ToolStripStatusLabel1.Text = ex.Message
        Catch ex As IOException
            ToolStripStatusLabel2.Text = ex.Message
        Catch ex As WebException
            ToolStripStatusLabel2.Text = ex.Message
        Finally
            If respuesta IsNot Nothing Then respuesta.Close()
            If fileStrm IsNot Nothing Then fileStrm.Close()
            If petiStrm IsNot Nothing Then petiStrm.Close()
        End Try

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            txt_pw.PasswordChar = ""
        Else
            txt_pw.PasswordChar = "*"
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        txt_server.Text = ""
        txt_user.Text = ""
        txt_pw.Text = ""
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ListBox1.Items.Clear()
            listarFTP(txt_server.Text, txt_user.Text, txt_pw.Text)
            MsgBox("Conexion establecida!")
        Catch ex As Exception
            MsgBox("Ha sido imposible conectar con el servidor!")
        End Try

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        descargarFTP(TextBox1.Text, txt_user.Text, txt_pw.Text)
        ListBox1.Items.Clear()
        listarFTP(txt_server.Text, txt_user.Text, txt_pw.Text)
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        TextBox1.Text = txt_server.Text & "/" & ListBox1.SelectedItem
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            If (MessageBox.Show("Esta seguro que quiere eliminar este archivo?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes) Then
                eliminarFTP(TextBox1.Text, txt_user.Text, txt_pw.Text)

            End If
            ListBox1.Items.Clear()
            listarFTP(txt_server.Text, txt_user.Text, txt_pw.Text)
        Catch ex As Exception
            MsgBox("Ha sido imposible eliminar el archivo!")

        End Try

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ListBox1.Items.Clear()
        listarFTP(txt_server.Text, txt_user.Text, txt_pw.Text)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            subirFTP(ListBox2.SelectedItem, txt_server.Text, txt_user.Text, txt_pw.Text)
            ListBox1.Items.Clear()
            listarFTP(txt_server.Text, txt_user.Text, txt_pw.Text)
        Catch ex As Exception
            MsgBox("El archivo no se ha podido subir!")
        End Try

        '   If (OpenFileDialog1.ShowDialog() = DialogResult.Yes) Then
        '  uploadFTP(OpenFileDialog1.FileName, txt_server.Text + "/" + Path.GetFileName(OpenFileDialog1.FileName), txt_user.Text, txt_pw.Text)
        ' End If

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBox2.Items.AddRange(Directory.GetDirectories("c:\"))


    End Sub

    Private Sub ListBox2_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox2.MouseDoubleClick

        TextBox2.Text = ListBox2.SelectedItem
        ListBox2.Items.Clear()
        ListBox2.Items.AddRange(Directory.GetDirectories(TextBox2.Text + "\"))
        ListBox2.Items.AddRange(Directory.GetFiles(TextBox2.Text + "\"))
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ListBox2.Items.Clear()
        ListBox2.Items.AddRange(Directory.GetDirectories("c:\"))
    End Sub
End Class
