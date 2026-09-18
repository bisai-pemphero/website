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
import com.myschool.app.ui.components.StatCard
import com.myschool.app.ui.theme.*

@Composable
fun DashboardScreen(
    userProfile: UserProfile,
    onLogout: () -> Unit,
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
                    IconButton(onClick = onLogout) {
                        Icon(
                            imageVector = Icons.Default.ExitToApp,
                            contentDescription = "Logout"
                        )
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primary,
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
                    }
                }
                is DashboardUiState.Error -> {
                    Text(
                        text = "Error loading stats: ${state.message}",
                        color = MaterialTheme.colorScheme.error
                    )
                }
                else -> {}
            }
            
            // Quick Actions based on role
            Text(
                text = "Quick Actions",
                style = MaterialTheme.typography.titleMedium
            )
            
            LazyVerticalGrid(
                columns = GridCells.Adaptive(minSize = 150.dp),
                horizontalArrangement = Arrangement.spacedBy(12.dp),
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                getQuickActionsForRole(userProfile.roleId).forEach { action ->
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

private fun getQuickActionsForRole(roleId: Int): List<QuickAction> {
    return when (roleId) {
        1 -> listOf(
            QuickAction("Schools", Icons.Default.Business, SystemAdminColor) {},
            QuickAction("Users", Icons.Default.People, SystemAdminColor) {},
            QuickAction("Reports", Icons.Default.Assessment, SystemAdminColor) {},
            QuickAction("Settings", Icons.Default.Settings, SystemAdminColor) {}
        )
        2 -> listOf(
            QuickAction("Staff", Icons.Default.People, DirectorColor) {},
            QuickAction("Students", Icons.Default.School, DirectorColor) {},
            QuickAction("Reports", Icons.Default.Assessment, DirectorColor) {},
            QuickAction("Fee Structure", Icons.Default.AttachMoney, DirectorColor) {}
        )
        3 -> listOf(
            QuickAction("Students", Icons.Default.School, HeadmasterColor) {},
            QuickAction("Academic", Icons.Default.CalendarMonth, HeadmasterColor) {},
            QuickAction("Exams", Icons.Default.Edit, HeadmasterColor) {},
            QuickAction("Classes", Icons.Default.Class_, HeadmasterColor) {},
            QuickAction("Teachers", Icons.Default.People, HeadmasterColor) {},
            QuickAction("Grading", Icons.Default.Star, HeadmasterColor) {}
        )
        4 -> listOf(
            QuickAction("Students", Icons.Default.School, BursarColor) {},
            QuickAction("Fees", Icons.Default.AttachMoney, BursarColor) {},
            QuickAction("Payments", Icons.Default.Payment, BursarColor) {},
            QuickAction("Receipts", Icons.Default.Receipt, BursarColor) {},
            QuickAction("Reports", Icons.Default.Assessment, BursarColor) {}
        )
        5 -> listOf(
            QuickAction("My Classes", Icons.Default.Class_, TeacherColor) {},
            QuickAction("Subjects", Icons.Default.Book, TeacherColor) {},
            QuickAction("Students", Icons.Default.School, TeacherColor) {},
            QuickAction("Exams", Icons.Default.Edit, TeacherColor) {},
            QuickAction("Results", Icons.Default.Star, TeacherColor) {}
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
