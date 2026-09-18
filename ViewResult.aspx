<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewResult.aspx.cs" Inherits="ViewResult" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Exam Portal</title>
<meta name="viewport" content="width=device-width, initial-scale=1" />

<style>
    :root {
        --primary: #0ea5e9;
        --primary-dark: #0369a1;
        --bg: #f1f5f9;
        --card: #ffffff;
        --text: #1e293b;
        --muted: #64748b;
        --radius: 16px;
        --shadow: 0 10px 30px rgba(0,0,0,.05);
    }

    * {
        box-sizing: border-box;
    }

    body {
        margin: 0;
        font-family: 'Segoe UI', Tahoma, sans-serif;
        background: linear-gradient(to bottom right, #f8fafc, #e2e8f0);
        color: var(--text);
    }

    /* Header */
    .header {
        background: white;
        padding: 18px 25px;
        box-shadow: 0 4px 12px rgba(0,0,0,.04);
        display: flex;
        justify-content: space-between;
        align-items: center;
        position: sticky;
        top: 0;
        z-index: 100;
    }

    .header h1 {
        margin: 0;
        font-size: 20px;
        font-weight: 600;
        color: var(--primary-dark);
    }

    /* Main Container */
    .container {
        padding: 30px 20px;
        max-width: 1200px;
        margin: auto;
    }

    .section-title {
        font-size: 22px;
        font-weight: 600;
        margin-bottom: 25px;
    }

    /* Grid Layout */
    .exam-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
        gap: 25px;
    }

    /* Card */
    .exam-card {
        background: var(--card);
        border-radius: var(--radius);
        padding: 25px;
        box-shadow: var(--shadow);
        transition: all .3s ease;
        border: 1px solid #e2e8f0;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }

    .exam-card:hover {
        transform: translateY(-6px);
        box-shadow: 0 15px 35px rgba(0,0,0,.08);
    }

    .exam-title {
        font-weight: 600;
        font-size: 18px;
        margin-bottom: 12px;
        color: var(--primary-dark);
    }

    .exam-meta {
        font-size: 14px;
        color: var(--muted);
        margin-bottom: 20px;
        line-height: 1.6;
    }

    /* Button */
    .btn-view {
        text-align: center;
        padding: 12px;
        background: linear-gradient(135deg, var(--primary), var(--primary-dark));
        color: white;
        text-decoration: none;
        border-radius: 12px;
        font-size: 14px;
        font-weight: 500;
        transition: all .3s ease;
    }

    .btn-view:hover {
        transform: translateY(-2px);
        opacity: .95;
    }

    /* Empty State */
    .empty {
        text-align: center;
        margin-top: 60px;
        color: var(--muted);
        font-size: 15px;
        padding: 40px;
        background: white;
        border-radius: var(--radius);
        box-shadow: var(--shadow);
    }

    /* Mobile Tweaks */
    @media (max-width: 600px) {
        .header h1 {
            font-size: 16px;
        }

        .section-title {
            font-size: 18px;
        }

        .exam-card {
            padding: 18px;
        }

        .btn-view {
            font-size: 13px;
            padding: 10px;
        }
    }

</style>
</head>

<body>

<form id="form1" runat="server">
<asp:ScriptManager runat="server" />

<!-- Header -->
<div class="header">
    <h1>🎓 Student Exam Portal</h1>
</div>

<div class="container">
    <div class="section-title">📚 My Exams</div>

    <asp:UpdatePanel runat="server">
        <ContentTemplate>

            <div class="exam-grid">
                <asp:Repeater ID="rptExams" runat="server">
                    <ItemTemplate>
                        <div class="exam-card">
                            <div>
                                <div class="exam-title">
                                    <%# Eval("Exam_name") %>
                                </div>

                                <div class="exam-meta">
                                    📅 Academic Year: <%# Eval("AcademicYear") %><br />
                                    🏫 Term: <%# Eval("TermName") %>
                                </div>
                            </div>

                          <asp:LinkButton ID="lnkViewResults" runat="server" 
                CommandArgument='<%# Eval("ExamsId") %>'
             
                OnCommand="lnkViewResults_Command"
                CssClass="btn-view">
    View Results →
</asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Label ID="lblEmpty" runat="server"
                CssClass="empty"
                Visible="false"
                Text="📭 No exams available at the moment. Please check back later.">
            </asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>
</div>

</form>
</body>
</html>
