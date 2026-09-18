<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Receipt.aspx.cs" Inherits="Test" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>Receipt</title>
    <script src="https://cdn.jsdelivr.net/npm/jsbarcode@3.11.6/dist/JsBarcode.all.min.js"></script>

    <style>
        body {
            font-family: "Courier New", monospace;
            font-size: 11px;
            margin: 0;
            padding: 0;
        }

        .receipt {
            width: 210px; /* 58mm roll */
            margin: 0 auto;
            padding: 5px;
        }

        .center {
            text-align: center;
        }

        .bold {
            font-weight: bold;
        }

        .divider {
            border-top: 1px dashed #000;
            margin: 6px 0;
        }

        .row {
            display: flex;
            justify-content: space-between;
            margin: 2px 0;
        }

        .label {
            text-align: left;
            flex: 1;
        }

        .value {
            text-align: right;
            flex: 1;
        }

        .logo {
            max-width: 40px;
            max-height: 40px;
            margin-bottom: 4px;
        }

        @media print {
            body {
                margin: 0;
            }
            .no-print {
                display: none;
            }
        }


    </style>


        <style>
    #barcode {
        max-width: 180px;
        height: auto;
        display: block;
        margin: 0 auto;
    }
</style>
    <script>
        function printReceipt() {
            window.print();
        }

        
    </script>
</head>
<body>
    <form runat="server">
        <div class="receipt">
            <br />
            <br />
            <div class="center">
                <img id="imgLogo" runat="server" src="" class="logo" alt="Logo" />
            <br />

                <div class="bold"><asp:Label runat="server" ID="lblSchoolName"></asp:Label></div>
                <div><asp:Label runat="server" ID="lblSlogan"></asp:Label></div>
                <div><asp:Label runat="server" ID="lblAddress"></asp:Label></div>
                <div><asp:Label runat="server" ID="lblPhoneNumber"></asp:Label></div>
            </div>
            <br />

            <div class="divider"></div>

            <div class="row"><span class="label">Receipt No:</span><span class="value"><asp:Label ID="lblReceiptNo" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Bursar:</span><span class="value"><asp:Label ID="lblUser" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Date:</span><span class="value"><asp:Label ID="lblDate" runat="server"></asp:Label></span></div>
            <br />

            <div class="divider"></div>

            <div class="row"><span class="label">Fees For:</span><span class="value"><asp:Label ID="lblFor" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Category:</span><span class="value"><asp:Label ID="lblCategory" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Paid By:</span><span class="value"><asp:Label ID="lblReceivedFrom" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Class:</span><span class="value"><asp:Label ID="lblClass" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Term:</span><span class="value"><asp:Label ID="lblterm" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Method:</span><span class="value"><asp:Label ID="lblmethod" runat="server"></asp:Label></span></div>

            <div class="divider"></div>

            <div class="row bold"><span class="label">Amount Paid:</span><span class="value"><asp:Label ID="lblamount" runat="server"></asp:Label></span></div>
            <div class="row"><span class="label">Balance:</span><span class="value"><asp:Label ID="lblbalance" runat="server"></asp:Label></span></div>

            <div class="divider"></div>



            <div style="text-align:center; margin-top:10px;">
    <svg id="barcode"></svg>
</div>

            
        </div>

        <div class="no-print center" style="margin-top:10px;">
            <input type="button" value="Print Receipt" onclick="printReceipt()" />
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" />
        </div>

        <asp:Label runat="server" ID="lblError"></asp:Label>
    </form>




   <script src="https://cdn.jsdelivr.net/npm/jsbarcode@3.11.6/dist/JsBarcode.all.min.js"></script>
    

<script>
    window.onload = function () {
        var receiptNo = document.getElementById("<%= lblReceiptNo.ClientID %>").innerText;

        JsBarcode("#barcode", receiptNo, {
            format: "CODE128",
            lineColor: "#000",
            width: 1.2,
            height: 35,
            displayValue: true,
            fontSize: 10,
            margin: 0
        });
    };
</script>

</body>
</html>


 
