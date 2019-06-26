﻿Public Class Clase_DePropanizador
    Private _D As Double
    Public Property D() As Double
        Get
            Return _D
        End Get
        Set(ByVal value As Double)
            _D = value
        End Set
    End Property
    Private _W As Double

    Public Property W() As Double
        Get
            Return _W
        End Get
        Set(ByVal value As Double)
            _W = value
        End Set
    End Property

    Private _Temp As Double
    Public Property TempCond() As Double
        Get
            Return _Temp
        End Get
        Set(ByVal value As Double)
            _Temp = value
        End Set
    End Property

    Public Sub BalanceMateria(ByVal num1 As NumericUpDown, ByVal num2 As NumericUpDown, ByVal num3 As NumericUpDown, ByVal num4 As NumericUpDown, ByVal num5 As NumericUpDown, ByVal num6 As NumericUpDown, ByVal num7 As NumericUpDown, ByVal numPropano As NumericUpDown, ByVal numButano As NumericUpDown, ByVal numD1 As NumericUpDown, ByVal numD2 As NumericUpDown, ByVal numD3 As NumericUpDown, ByVal numW1 As NumericUpDown, ByVal numW2 As NumericUpDown, ByVal numW3 As NumericUpDown, ByVal numW4 As NumericUpDown, ByVal numW5 As NumericUpDown, ByVal numW6 As NumericUpDown)
        _D = 0
        _W = 0
        'Calculo de flujos
        _D = num1.Value + (numPropano.Value / 100) * num2.Value + (numButano.Value / 100) * num3.Value
        _W = 100 - _D
        'Calculo de composiciones en Overhead
        If (_D > 0) Then
            numD1.Value = num1.Value * 100D / _D
            numD2.Value = (numPropano.Value * num2.Value) / _D
            numD3.Value = 100D - (numD1.Value + numD2.Value)
        Else
            numD1.Value = 0.0D
            numD2.Value = 0.0D
            numD3.Value = 0.0D
        End If
        'Calculo de composiciones en Bottom
        If (_W > 0) Then
            numW1.Value = ((100D - numPropano.Value) * num2.Value) / _W
            numW2.Value = ((100D - numButano.Value) * num3.Value) / _W
            numW3.Value = num4.Value * 100 / _W
            numW4.Value = num5.Value * 100 / _W
            numW5.Value = num6.Value * 100 / _W
            numW6.Value = 100D - (numW1.Value + numW2.Value + numW3.Value + numW4.Value + numW5.Value)
        Else
            numW1.Value = 0.0D
            numW2.Value = 0.0D
            numW3.Value = 0.0D
            numW4.Value = 0.0D
            numW5.Value = 0.0D
            numW6.Value = 0.0D
        End If
    End Sub
    Public Sub CalculoK(ByVal numD1 As NumericUpDown, ByVal numD2 As NumericUpDown, ByVal numD3 As NumericUpDown, ByVal numW1 As NumericUpDown, ByVal numW2 As NumericUpDown, ByVal numW3 As NumericUpDown, ByVal numW4 As NumericUpDown, ByVal numW5 As NumericUpDown, ByVal numW6 As NumericUpDown)
        ' Por la correlacion de Standing C2 - C6
        ' log10(Ki*P)=a+c*Fi <- Existe linealidad!
        ' Ver: Hidrocarbon Phase Behavior - Amed Tarek pag. 251
        ' Donde: Temp [°F]
        _Temp += 460 ' Convertimos a Rankine
#Region "Constantes necesarias para la correlacion"
        ' Notacion:
        '   C2   0
        '   C3   1
        '   iC4  2
        '   nC4  3
        '   iC5  4
        '   nC5  5
        '   C6   6
        ' bi
        Dim bi() As Double = {1.145, 1.799, 2.037, 2.153, 2.368, 2.48, 2.738}

        ' Tbi °R
        Dim Tbi() As Integer = {303, 416, 471, 491, 542, 557, 610}
#End Region
        ' Declaracion de variables
        Dim i As UInteger = 0, j As UInteger = 0, k As UInteger
        Dim Pres As Double = 0
        Dim TempW As Double = _Temp
        Dim a As Double = 0, c As Double = 0
        Dim sumPresBurbuj As Double = 0, sumTempBurbuj As Double = 0
        ' Destilado
        Dim KiD(2) As Double
        Dim FiD(2) As Double
        'Dim FiD = New Double() {C2bi * (1 / C2Tbi + 1 / Temp), C3bi * (1 / C3Tbi + 1 / Temp), iC4bi * (1 / iC4Tbi + 1 / Temp)}
        Dim CompD = New Double() {numD1.Value / 100, numD2.Value / 100, numD3.Value / 100}
        'Colas
        Dim KiW(5) As Double
        Dim FiW(5) As Double
        Dim CompW = New Double() {numW1.Value / 100, numW2.Value / 100, numW3.Value / 100, numW4.Value / 100, numW4.Value / 100, numW5.Value / 100, numW6.Value / 100}

        'Calculo en "D"/ Presion de burbuja
        Pres = 250.0
        For k = 0 To 2
            FiD(k) = bi(k) * (1 / Tbi(k) + 1 / _Temp)
        Next
        Do While (True)
            sumPresBurbuj = 0
            a = 1.2 + 0.0045 * Pres + 0.00000015 * Math.Pow(Pres, 2)
            c = 0.89 - 0.00017 * Pres - 0.000000035 * Math.Pow(Pres, 2)
            For i = 0 To 2
                KiD(i) = Math.Pow(10, a + c * FiD(i)) / Pres
            Next
            For j = 0 To 2
                sumPresBurbuj = sumPresBurbuj + CompD(j) * KiD(j)
                MsgBox(" sumPres00: " & sumPresBurbuj & " " & j)
                'Problema sumPres = 0
            Next
            MsgBox(" sumPres00: " & sumPresBurbuj & " " & Pres)
            If (sumPresBurbuj > 0.99 And sumPresBurbuj < 1.01) Then
                'Salir
                MsgBox(" sumPres: " & sumPresBurbuj)
                Exit Do
            ElseIf sumPresBurbuj >= 1.01 Then
                Pres *= 1.1
            ElseIf sumPresBurbuj <= 0.99 Then
                Pres *= 0.9
            Else
                MsgBox("Caso no definido")
            End If
        Loop

        ' Calculo en "W" / Temperatura de Burbuja
        Do While (True)
            sumTempBurbuj = 0
            For i = 0 To 5
                FiW(i) = bi(i + 1) * (1 / Tbi(i + 1) + 1 / TempW)
            Next
            For j = 0 To 5
                KiW(j) = Math.Pow(10, a + c * FiW(j)) / Pres
            Next
            For k = 0 To 5
                sumTempBurbuj += CompW(k) * KiW(k)
            Next
            If (sumTempBurbuj > 0.99 And sumTempBurbuj < 1.01) Then
                'Salir
                MsgBox("TempW. " & TempW & vbCrLf & sumTempBurbuj)
                Exit Do
            ElseIf (sumTempBurbuj >= 1.01) Then
                TempW *= 1.1
                MsgBox("TempW. " & TempW & vbCrLf & sumTempBurbuj)
            ElseIf (sumTempBurbuj <= 0.99) Then
                TempW *= 0.9
                MsgBox("TempW. " & TempW & vbCrLf & sumTempBurbuj)
            Else
                MsgBox("Caso no definido")
                Exit Sub
            End If
        Loop
        MsgBox("Metodo terminado!")
    End Sub
End Class