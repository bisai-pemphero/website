package com.myschool.app.navigation

import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.ui.graphics.vector.ImageVector

sealed class Screen(val route: String, val title: String, val icon: ImageVector? = null) {
    object Login : Screen("login", "Login")
    object Dashboard : Screen("dashboard", "Dashboard", Icons.Default.Dashboard)
    
    // SystemAdmin Screens
    object Schools : Screen("schools", "Schools", Icons.Default.Business)
    object AllUsers : Screen("users", "Users", Icons.Default.People)
    
    // Common Screens (all roles)
    object Profile : Screen("profile", "Profile", Icons.Default.Person)
    object Settings : Screen("settings", "Settings", Icons.Default.Settings)
    
    // Director Screens
    object Staff : Screen("staff", "Staff", Icons.Default.People)
    object FeeStructure : Screen("fee-structure", "Fee Structure", Icons.Default.AttachMoney)
    
    // Headmaster Screens
    object Students : Screen("students", "Students", Icons.Default.School)
    object AcademicYears : Screen("academic-years", "Academic Years", Icons.Default.CalendarMonth)
    object Terms : Screen("terms", "Terms", Icons.Default.Event)
    object Exams : Screen("exams", "Exams", Icons.Default.Edit)
    object Subjects : Screen("subjects", "Subjects", Icons.Default.Book)
    object Classes : Screen("classes", "Classes", Icons.Default.Class_)
    object TeacherAssignments : Screen("teacher-assignments", "Teacher Assignments", Icons.Default.PeopleOutline)
    object ClassTeachers : Screen("class-teachers", "Class Teachers", Icons.Default.Badge)
    object GradingSystem : Screen("grading-system", "Grading System", Icons.Default.Star)
    object StudentPromotion : Screen("student-promotion", "Student Promotion", Icons.Default.SwapHoriz)
    
    // Bursar Screens
    object Fees : Screen("fees", "Fees", Icons.Default.AttachMoney)
    object Payments : Screen("payments", "Payments", Icons.Default.Payment)
    object Receipts : Screen("receipts", "Receipts", Icons.Default.Receipt)
    object OutstandingBalances : Screen("outstanding-balances", "Outstanding Balances", Icons.Default.TrendingUp)
    
    // Teacher Screens
    object MyClasses : Screen("my-classes", "My Classes", Icons.Default.Class_)
    object MySubjects : Screen("my-subjects", "My Subjects", Icons.Default.Book)
    object EnterMarks : Screen("enter-marks", "Enter Marks", Icons.Default.EditNote)
    object ViewResults : Screen("view-results", "View Results", Icons.Default.Assessment)
    
    // Reports (available to multiple roles)
    object Reports : Screen("reports", "Reports", Icons.Default.Assessment)
    object StudentListReport : Screen("student-list-report", "Student List")
    object FeeCollectionReport : Screen("fee-collection-report", "Fee Collection Report")
}

data class NavItem(
    val screen: Screen,
    val allowedRoles: List<Int> // Role IDs that can access this screen
)

object NavigationGraph {
    // Role IDs: 1=SystemAdmin, 2=Director, 3=Headmaster, 4=Bursar, 5=Teacher
    
    fun getNavigationItemsForRole(roleId: Int): List<NavItem> {
        return when (roleId) {
            1 -> listOf( // SystemAdmin
                NavItem(Screen.Dashboard, listOf(1)),
                NavItem(Screen.Schools, listOf(1)),
                NavItem(Screen.AllUsers, listOf(1)),
                NavItem(Screen.Reports, listOf(1, 2, 4)),
                NavItem(Screen.Profile, listOf(1, 2, 3, 4, 5)),
                NavItem(Screen.Settings, listOf(1))
            )
            2 -> listOf( // Director
                NavItem(Screen.Dashboard, listOf(2)),
                NavItem(Screen.Staff, listOf(2)),
                NavItem(Screen.Students, listOf(2, 3, 4, 5)),
                NavItem(Screen.FeeStructure, listOf(2)),
                NavItem(Screen.Reports, listOf(1, 2, 4)),
                NavItem(Screen.Profile, listOf(1, 2, 3, 4, 5))
            )
            3 -> listOf( // Headmaster
                NavItem(Screen.Dashboard, listOf(3)),
                NavItem(Screen.Students, listOf(2, 3, 4, 5)),
                NavItem(Screen.AcademicYears, listOf(3)),
                NavItem(Screen.Terms, listOf(3)),
                NavItem(Screen.Exams, listOf(3, 5)),
                NavItem(Screen.Subjects, listOf(3)),
                NavItem(Screen.Classes, listOf(3)),
                NavItem(Screen.TeacherAssignments, listOf(3)),
                NavItem(Screen.ClassTeachers, listOf(3)),
                NavItem(Screen.GradingSystem, listOf(3)),
                NavItem(Screen.StudentPromotion, listOf(3)),
                NavItem(Screen.Reports, listOf(1, 2, 4)),
                NavItem(Screen.Profile, listOf(1, 2, 3, 4, 5))
            )
            4 -> listOf( // Bursar
                NavItem(Screen.Dashboard, listOf(4)),
                NavItem(Screen.Students, listOf(2, 3, 4, 5)),
                NavItem(Screen.Fees, listOf(4)),
                NavItem(Screen.Payments, listOf(4)),
                NavItem(Screen.Receipts, listOf(4)),
                NavItem(Screen.OutstandingBalances, listOf(4)),
                NavItem(Screen.FeeCollectionReport, listOf(4)),
                NavItem(Screen.Profile, listOf(1, 2, 3, 4, 5))
            )
            5 -> listOf( // Teacher
                NavItem(Screen.Dashboard, listOf(5)),
                NavItem(Screen.MyClasses, listOf(5)),
                NavItem(Screen.MySubjects, listOf(5)),
                NavItem(Screen.EnterMarks, listOf(5)),
                NavItem(Screen.ViewResults, listOf(5)),
                NavItem(Screen.Students, listOf(2, 3, 4, 5)),
                NavItem(Screen.Profile, listOf(1, 2, 3, 4, 5))
            )
            else -> emptyList()
        }
    }
    
    fun getBottomNavItemsForRole(roleId: Int): List<NavItem> {
        return getNavigationItemsForRole(roleId).filter { navItem ->
            navItem.screen.icon != null && 
            navItem.screen !is Screen.Profile && 
            navItem.screen !is Screen.Settings
        }.take(5) // Limit to 5 items for bottom navigation
    }
}
