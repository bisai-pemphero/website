package com.myschool.app.ui.screens.dashboard

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import com.myschool.app.domain.model.UserProfile
import com.myschool.app.navigation.Screen
import com.myschool.app.ui.components.StatCard
import com.myschool.app.ui.theme.*

@Composable
fun DashboardScreen(
    userProfile: UserProfile,
    onLogout: () -> Unit,
    onNavigateToScreen: (String) -> Unit,
    modifier: Modifier = Modifier,
    viewModel: DashboardViewModel = hiltViewModel()
) {
    val uiState by viewModel.uiState.collectAsState()
    
    Scaffold(
        topBar = {
            TopAppBar(
                title = { 
                    Text(
                        text = "Dashboard",
                        style = MaterialTheme.typography.titleLarge
                    )
                },
                actions = {
                    IconButton(onClick = { /* Refresh */ viewModel.refresh() }) {
                        Icon(
                            imageVector = Icons.Default.Refresh,
                            contentDescription = "Refresh"
                        )
                    }
                    IconButton(onClick = onLogout) {
                        Icon(
                            imageVector = Icons.Default.ExitToApp,
                            contentDescription = "Logout"
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = getRoleColor(userProfile.roleId),
                    titleContentColor = MaterialTheme.colorScheme.onPrimary
                )
            )
        }
    ) { paddingValues ->
        Column(
            modifier = modifier
                .fillMaxSize()
                .padding(paddingValues)
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(24.dp)
        ) {
            // Welcome Section
            Card(
                modifier = Modifier.fillMaxWidth(),
                elevation = CardDefaults.cardElevation(defaultElevation = 2.dp),
                shape = MaterialTheme.shapes.large,
                colors = CardDefaults.cardColors(
                    containerColor = getRoleColor(userProfile.roleId)
                )
            ) {
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(24.dp),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    Text(
                        text = "Welcome back,",
                        style = MaterialTheme.typography.bodyLarge,
                        color = MaterialTheme.colorScheme.onPrimary.copy(alpha = 0.9f)
                    )
                    Text(
                        text = userProfile.username,
                        style = MaterialTheme.typography.headlineMedium,
                        color = MaterialTheme.colorScheme.onPrimary
                    )
                    Row(
                        horizontalArrangement = Arrangement.spacedBy(8.dp),
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Badge(
                            containerColor = MaterialTheme.colorScheme.onPrimary.copy(alpha = 0.3f)
                        ) {
                            Text(
                                text = getRoleName(userProfile.roleId),
                                style = MaterialTheme.typography.labelMedium,
                                color = MaterialTheme.colorScheme.onPrimary
                            )
                        }
                        if (userProfile.schoolName != null) {
                            Text(
                                text = "• ${userProfile.schoolName}",
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onPrimary.copy(alpha = 0.8f)
                            )
                        }
                    }
                }
            }
            
            // Stats Grid
            when (val state = uiState) {
                is DashboardUiState.Loading -> {
                    Box(
                        modifier = Modifier.fillMaxWidth().height(200.dp),
                        contentAlignment = Alignment.Center
                    ) {
                        CircularProgressIndicator()
                    }
                }
                is DashboardUiState.Success -> {
                    val stats = state.stats
                    LazyVerticalGrid(
                        columns = GridCells.Fixed(2),
                        horizontalArrangement = Arrangement.spacedBy(12.dp),
                        verticalArrangement = Arrangement.spacedBy(12.dp),
                        modifier = Modifier.weight(1f)
                    ) {
                        item {
                            StatCard(
                                title = "Students",
                                value = stats.totalStudents.toString(),
                                icon = {
                                    Icon(
                                        imageVector = Icons.Default.School,
                                        contentDescription = null,
                                        tint = PrimaryBlue
                                    )
                                }
                            )
                        }
                        item {
                            StatCard(
                                title = "Teachers",
                                value = stats.totalTeachers.toString(),
                                icon = {
                                    Icon(
                                        imageVector = Icons.Default.People,
                                        contentDescription = null,
                                        tint = SecondaryTeal
                                    )
                                }
                            )
                        }
                        item {
                            StatCard(
                                title = "Classes",
                                value = stats.totalClasses.toString(),
                                icon = {
                                    Icon(
                                        imageVector = Icons.Default.Class_,
                                        contentDescription = null,
                                        tint = HeadmasterColor
                                    )
                                }
                            )
                        }
                        item {
                            StatCard(
                                title = "Fees Collected",
                                value = "K${stats.totalFeesCollected.toInt()}",
                                icon = {
                                    Icon(
                                        imageVector = Icons.Default.AttachMoney,
                                        contentDescription = null,
                                        tint = SuccessGreen
                                    )
                                }
                            )
                        }
                        item {
                            StatCard(
                                title = "Outstanding",
                                value = "K${stats.outstandingFees.toInt()}",
                                icon = {
                                    Icon(
                                        imageVector = Icons.Default.TrendingUp,
                                        contentDescription = null,
                                        tint = WarningOrange
                                    )
                                }
                            )
                        }
                        if (state.stats.activeAcademicYear != null) {
                            item {
                                Card(
                                    modifier = Modifier
                                        .aspectRatio(1.5f)
                                        .fillMaxWidth(),
                                    elevation = CardDefaults.cardElevation(defaultElevation = 2.dp),
                                    shape = MaterialTheme.shapes.medium,
                                    colors = CardDefaults.cardColors(
                                        containerColor = MaterialTheme.colorScheme.surfaceVariant
                                    )
                                ) {
                                    Column(
                                        modifier = Modifier
                                            .fillMaxSize()
                                            .padding(16.dp),
                                        horizontalAlignment = Alignment.CenterHorizontally,
                                        verticalArrangement = Arrangement.Center
                                    ) {
                                        Icon(
                                            imageVector = Icons.Default.CalendarMonth,
                                            contentDescription = null,
                                            modifier = Modifier.size(32.dp),
                                            tint = MaterialTheme.colorScheme.primary
                                        )
                                        Spacer(modifier = Modifier.height(8.dp))
                                        Text(
                                            text = "Academic Year",
                                            style = MaterialTheme.typography.labelMedium,
                                            color = MaterialTheme.colorScheme.onSurfaceVariant
                                        )
                                        Text(
                                            text = state.stats.activeAcademicYear ?: "N/A",
                                            style = MaterialTheme.typography.titleSmall,
                                            color = MaterialTheme.colorScheme.primary
                                        )
                                    }
                                }
                            }
                        }
                    }
                }
                is DashboardUiState.Error -> {
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.errorContainer
                        )
                    ) {
                        Text(
                            text = "Error loading stats: ${state.message}",
                            modifier = Modifier.padding(16.dp),
                            color = MaterialTheme.colorScheme.onErrorContainer
                        )
                    }
                }
                else -> {}
            }
            
            // Quick Actions based on role
            Text(
                text = "Quick Actions",
                style = MaterialTheme.typography.titleMedium
            )
            
            LazyVerticalGrid(
                columns = GridCells.Adaptive(minSize = 140.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp),
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                getQuickActionsForRole(userProfile.roleId, onNavigateToScreen).forEach { action ->
                    item {
                        ActionCard(
                            title = action.title,
                            icon = action.icon,
                            color = action.color,
                            onClick = action.onClick
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun ActionCard(
    title: String,
    icon: ImageVector,
    color: androidx.compose.ui.graphics.Color,
    onClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .aspectRatio(1f)
            .fillMaxWidth(),
        onClick = onClick,
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp),
        shape = MaterialTheme.shapes.medium,
        colors = CardDefaults.cardColors(
            containerColor = color.copy(alpha = 0.1f)
        )
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center
        ) {
            Icon(
                imageVector = icon,
                contentDescription = null,
                modifier = Modifier.size(40.dp),
                tint = color
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = title,
                style = MaterialTheme.typography.labelMedium,
                color = color
            )
        }
    }
}

private fun getRoleColor(roleId: Int): androidx.compose.ui.graphics.Color {
    return when (roleId) {
        1 -> SystemAdminColor
        2 -> DirectorColor
        3 -> HeadmasterColor
        4 -> BursarColor
        5 -> TeacherColor
        else -> PrimaryBlue
    }
}

private fun getRoleName(roleId: Int): String {
    return when (roleId) {
        1 -> "System Admin"
        2 -> "Director"
        3 -> "Headmaster"
        4 -> "Bursar"
        5 -> "Teacher"
        else -> "User"
    }
}

private fun getQuickActionsForRole(roleId: Int, onNavigateToScreen: (String) -> Unit): List<QuickAction> {
    return when (roleId) {
        1 -> listOf(
            QuickAction("Schools", Icons.Default.Business, SystemAdminColor) { onNavigateToScreen(Screen.Schools.route) },
            QuickAction("Users", Icons.Default.People, SystemAdminColor) { onNavigateToScreen(Screen.AllUsers.route) },
            QuickAction("Reports", Icons.Default.Assessment, SystemAdminColor) { onNavigateToScreen(Screen.Reports.route) },
            QuickAction("Settings", Icons.Default.Settings, SystemAdminColor) { onNavigateToScreen(Screen.Settings.route) }
        )
        2 -> listOf(
            QuickAction("Staff", Icons.Default.People, DirectorColor) { onNavigateToScreen(Screen.Staff.route) },
            QuickAction("Students", Icons.Default.School, DirectorColor) { onNavigateToScreen(Screen.Students.route) },
            QuickAction("Reports", Icons.Default.Assessment, DirectorColor) { onNavigateToScreen(Screen.Reports.route) },
            QuickAction("Fee Structure", Icons.Default.AttachMoney, DirectorColor) { onNavigateToScreen(Screen.FeeStructure.route) }
        )
        3 -> listOf(
            QuickAction("Students", Icons.Default.School, HeadmasterColor) { onNavigateToScreen(Screen.Students.route) },
            QuickAction("Academic", Icons.Default.CalendarMonth, HeadmasterColor) { onNavigateToScreen(Screen.AcademicYears.route) },
            QuickAction("Exams", Icons.Default.Edit, HeadmasterColor) { onNavigateToScreen(Screen.Exams.route) },
            QuickAction("Classes", Icons.Default.Class_, HeadmasterColor) { onNavigateToScreen(Screen.Classes.route) },
            QuickAction("Teachers", Icons.Default.People, HeadmasterColor) { onNavigateToScreen(Screen.TeacherAssignments.route) },
            QuickAction("Grading", Icons.Default.Star, HeadmasterColor) { onNavigateToScreen(Screen.GradingSystem.route) }
        )
        4 -> listOf(
            QuickAction("Students", Icons.Default.School, BursarColor) { onNavigateToScreen(Screen.Students.route) },
            QuickAction("Fees", Icons.Default.AttachMoney, BursarColor) { onNavigateToScreen(Screen.Fees.route) },
            QuickAction("Payments", Icons.Default.Payment, BursarColor) { onNavigateToScreen(Screen.Payments.route) },
            QuickAction("Receipts", Icons.Default.Receipt, BursarColor) { onNavigateToScreen(Screen.Receipts.route) },
            QuickAction("Reports", Icons.Default.Assessment, BursarColor) { onNavigateToScreen(Screen.FeeCollectionReport.route) }
        )
        5 -> listOf(
            QuickAction("My Classes", Icons.Default.Class_, TeacherColor) { onNavigateToScreen(Screen.MyClasses.route) },
            QuickAction("Subjects", Icons.Default.Book, TeacherColor) { onNavigateToScreen(Screen.MySubjects.route) },
            QuickAction("Students", Icons.Default.School, TeacherColor) { onNavigateToScreen(Screen.Students.route) },
            QuickAction("Exams", Icons.Default.Edit, TeacherColor) { onNavigateToScreen(Screen.Exams.route) },
            QuickAction("Results", Icons.Default.Star, TeacherColor) { onNavigateToScreen(Screen.ViewResults.route) }
        )
        else -> emptyList()
    }
}

data class QuickAction(
    val title: String,
    val icon: ImageVector,
    val color: androidx.compose.ui.graphics.Color,
    val onClick: () -> Unit
)
