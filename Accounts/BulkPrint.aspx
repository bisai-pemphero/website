<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BulkPrint.aspx.cs" Inherits="BulkPrint" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bulk Receipt Printing - Thermal</title>
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Courier New', monospace;
            font-size: 11px;
            line-height: 1.2;
            background: white;
            width: 58mm;
            margin: 0 auto;
            padding: 2mm;
        }

        .receipt {
            width: 58mm;
            min-height: 50mm;
            padding: 2mm;
            margin: 0 auto 3mm auto;
            border: 1px dashed #ccc;
            page-break-after: always;
            break-after: always;
        }

            .receipt:last-child {
                page-break-after: auto;
                break-after: auto;
            }

        .header {
            text-align: center;
            margin-bottom: 2mm;
            padding-bottom: 1mm;
            border-bottom: 1px dashed #000;
        }

        .logo {
            max-width: 40px;
            max-height: 40px;
            margin-bottom: 4px;
        }

        .school-name {
            font-weight: bold;
            font-size: 11px;
            margin-bottom: 1mm;
            text-transform: uppercase;
        }

        .school-address {
            font-size: 8px;
            margin-bottom: 1mm;
        }

        .school-contact {
            font-size: 8px;
            margin-bottom: 1mm;
        }

        .receipt-title {
            font-weight: bold;
            font-size: 11px;
            text-transform: uppercase;
            margin: 2mm 0;
        }

        .details {
            margin: 2mm 0;
        }

        .detail-line {
            display: flex;
            justify-content: space-between;
            margin: 1mm 0;
            font-size: 9px;
        }

        .label {
            font-weight: bold;
            text-align: left;
        }

        .value {
            text-align: right;
            flex: 1;
            margin-left: 2mm;
        }

        .separator {
            border-top: 1px dashed #000;
            margin: 2mm 0;
        }

        .amount-line {
            font-weight: bold;
            font-size: 10px;
            margin: 2mm 0;
        }

        .footer {
            text-align: center;
            margin-top: 3mm;
            padding-top: 2mm;
            border-top: 1px dashed #000;
            font-size: 8px;
            font-style: italic;
        }

        .barcode-container {
            text-align: center;
            margin: 2mm 0;
        }

        .barcode {
            max-width: 100%;
            height: auto;
        }

        .thank-you {
            text-align: center;
            font-weight: bold;
            margin: 2mm 0;
            font-size: 9px;
        }

        /* Print specific styles */
        @media print {
            body {
                width: 58mm;
                margin: 0;
                padding: 0;
                background: white;
            }

            .receipt {
                border: none;
                margin: 0 auto;
                padding: 2mm;
                page-break-after: always;
                break-after: always;
            }

                .receipt:last-child {
                    page-break-after: auto;
                    break-after: auto;
                }

            .no-print {
                display: none !important;
            }
        }

        /* Screen only styles */
        @media screen {
            body {
                background: #f0f0f0;
                padding: 10px;
            }

            .controls {
                text-align: center;
                margin: 10px 0;
                padding: 10px;
                background: #e9ecef;
                border-radius: 3px;
            }

            .btn {
                padding: 8px 15px;
                margin: 2px;
                background: #007bff;
                color: white;
                border: none;
                border-radius: 3px;
                cursor: pointer;
                font-size: 10px;
            }

            .btn-print {
                background: #28a745;
            }

            .btn-back {
                background: #6c757d;
            }

            .summary {
                text-align: center;
                margin: 10px 0;
                padding: 8px;
                background: #d1ecf1;
                border-radius: 3px;
                font-size: 9px;
            }

            .error {
                color: red;
                text-align: center;
                padding: 8px;
                margin: 8px 0;
                background: #ffe6e6;
                border: 1px solid red;
                font-size: 9px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Screen Only Controls -->
        <div class="no-print">
            <div class="summary">
                <asp:Label ID="lblSummary" runat="server" Text="" Font-Size="Large"></asp:Label>
            </div>

            <div class="controls">
                <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-print" OnClientClick="printThermalReceipts(); return false;" />
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-back" OnClick="btnBack_Click" />
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error" Visible="false"></asp:Label>
        </div>

        <!-- Receipts Container -->
        <div id="receiptsContainer" runat="server">
            <!-- Receipts will be dynamically generated here -->
        </div>
    </form>

    <script type="text/javascript">
        function printThermalReceipts() {
            // Set thermal printer specific settings
            var printContent = document.getElementById('receiptsContainer').innerHTML;
            var originalContent = document.body.innerHTML;

            document.body.innerHTML = printContent;
            window.print();
            document.body.innerHTML = originalContent;

            // Reload to restore original state
            window.location.reload();
        }

        // Auto-print when page loads if specified in query string
        window.onload = function () {
            var urlParams = new URLSearchParams(window.location.search);
            if (urlParams.get('autoprint') === 'true') {
                setTimeout(printThermalReceipts, 500);
            }
        };
    </script>
</body>
</html>
