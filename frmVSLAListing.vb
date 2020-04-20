﻿Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmVSLAListing
    Private Sub frmVSLAListing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
        fillgrid()
    End Sub

    Private Sub populatecounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT DISTINCT wards.county_id, wards.county " &
                                        "From   wards INNER JOIN " &
                                                         "OVCRegistrationDetails ON wards.county_id = OVCRegistrationDetails.countyid " &
                                                        " where OVCRegistrationDetails.cbo_id in (" & strcbos & ") ORDER BY wards.county"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbocounty
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "county"
                .ValueMember = "county_id"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub populatewards(ByVal strcounty_id As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct ward_id,ward from wards  " &
                " where county_id = '" & strcounty_id.ToString & "' order by ward"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbowards
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "ward"
                .ValueMember = "ward_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from vsla_list order by vsla_name,county,ward"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myvslaid, myvsla, myward, mycounty, mydateofregistration As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myvslaid = MyDatable.Rows(0).Item("vslaid").ToString
                    myvsla = MyDatable.Rows(0).Item("vsla_name").ToString
                    mycounty = MyDatable.Rows(0).Item("county").ToString
                    myward = MyDatable.Rows(0).Item("ward").ToString
                    mydateofregistration = MyDatable.Rows(0).Item("date_of_formation").ToString
                    DataGridView1.Rows.Add(myvslaid, myvsla, mycounty, myward, mydateofregistration, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("VSLAListing", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction =
            "INSERT INTO [dbo].[vsla_list] " &
           "([vsla_name] " &
           ",[date_of_formation]" &
           ",[Is_Gov_registered]" &
           ",[registration_date_gov]" &
           ",[ward_id] " &
           ",[ward] " &
           ",[county_id] " &
           ",[county] " &
           ",[chairperson] " &
           ",[chairperson_phonenumber] " &
           ",[Is_Directly_Monitored]) " &
            "VALUES " &
           "( '" & txtName.Text.ToString & "'," &
          "'" & Format(dtpDateofformation.Value, "dd-MMM-yyyy") & "'," &
          "'" & chkIsGovRegistered.Checked & "'," &
          "'" & Format(dtpDateRegisteredGov.Value, "dd-MMM-yyyy") & "'," &
          " '" & cbowards.SelectedValue.ToString & "'," &
          " '" & cbowards.Text.ToString & "'," &
          " '" & cbocounty.SelectedValue.ToString & "'," &
           "'" & cbocounty.Text.ToString & "'," &
           "'" & txtchairpersonname.Text.ToString & "'," &
           "'" & txtchairpersonnumber.Text.ToString & "',  " &
            "'" & chkIsDirectlyMonitored.Checked & "')   "




            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vslaListing", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub cbocounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedValueChanged
        Try

            If IsNumeric(cbocounty.SelectedValue) = True Then
                populatewards(cbocounty.SelectedValue.ToString)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cbocounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedIndexChanged

    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex


            Dim MyDatable As New Data.DataTable
            mysqlaction = "select * from vsla_list where vslaid=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtVSLAID.Text = MyDatable.Rows(0).Item("vslaid").ToString
                txtName.Text = MyDatable.Rows(0).Item("vsla_name").ToString
                cbocounty.SelectedValue = MyDatable.Rows(0).Item("county_ID").ToString
                cbowards.SelectedValue = MyDatable.Rows(0).Item("ward_ID").ToString
                dtpDateofformation.Value = CDate(MyDatable.Rows(0).Item("date_of_formation").ToString)
                dtpDateRegisteredGov.Value = CDate(MyDatable.Rows(0).Item("registration_date_gov").ToString)
                chkIsGovRegistered.Checked = CBool(MyDatable.Rows(0).Item("Is_Gov_registered"))
                txtchairpersonname.Text = MyDatable.Rows(0).Item("chairperson").ToString
                txtchairpersonnumber.Text = MyDatable.Rows(0).Item("chairperson_phonenumber").ToString
                chkIsDirectlyMonitored.Checked = CBool(MyDatable.Rows(0).Item("Is_Directly_Monitored"))
            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnEdit_Click_1(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "UPDATE [dbo].[vsla_list] " &
                               "SET [vsla_name] = '" & txtName.Text.ToString & "' " &
                                 " ,[date_of_formation] = '" & Format(dtpDateofformation.Value, "dd-MMM-yyyy") & "' " &
                                  ",[Is_Gov_registered] = '" & chkIsGovRegistered.Checked & "' " &
                                  " ,[registration_date_gov] = '" & Format(dtpDateRegisteredGov.Value, "dd-MMM-yyyy") & "' " &
                                  ",[ward_id] = '" & cbowards.SelectedValue.ToString & "' " &
                                 " ,[ward] = '" & cbowards.Text.ToString & "' " &
                                 " ,[county_id] = '" & cbocounty.SelectedValue.ToString & "' " &
                                 " ,[county] = '" & cbocounty.Text.ToString & "' " &
                                  ",[chairperson] = '" & txtchairpersonname.Text.ToString & "' " &
                                 " ,[chairperson_phonenumber] = '" & txtName.Text.ToString & "' " &
                                 " ,[Is_Directly_Monitored] = '" & chkIsDirectlyMonitored.Checked & "' " &
                             "WHERE vslaid = '" & txtVSLAID.Text.ToString & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            BtnEdit.Enabled = False
            BtnDelete.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("VSLAListing", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles BtnDelete.Click

    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        clearcontrols()
        Panel1.Enabled = True
        btnpost.Enabled = True
    End Sub

    Private Sub clearcontrols()
        Try


            'to clear textBoxes,radiobutton and checkboxes
            ' that are in containers 
            Dim ctrl As Control = Me.GetNextControl(Me, True)
            Do Until ctrl Is Nothing
                If TypeOf ctrl Is TextBox Then
                    ctrl.Text = String.Empty
                ElseIf TypeOf ctrl Is RadioButton Then
                    DirectCast(ctrl, RadioButton).Checked = False
                ElseIf TypeOf ctrl Is CheckBox Then
                    DirectCast(ctrl, CheckBox).Checked = False
                ElseIf TypeOf ctrl Is ComboBox Then
                    DirectCast(ctrl, ComboBox).SelectedIndex = -1
                ElseIf TypeOf ctrl Is DateTimePicker Then
                    DirectCast(ctrl, DateTimePicker).Value = Date.Today
                ElseIf TypeOf ctrl Is MaskedTextBox Then
                    ctrl.Text = String.Empty
                End If

                ctrl = Me.GetNextControl(ctrl, True)

            Loop

        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()

    End Sub

    Private Sub cbocounty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbocounty.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbowards_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbowards.SelectedIndexChanged

    End Sub

    Private Sub cbowards_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbowards.KeyPress
        e.Handled = True
    End Sub
End Class