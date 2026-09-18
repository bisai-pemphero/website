package com.myschool.app.ui.screens.main

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.myschool.app.domain.model.UserProfile
import com.myschool.app.navigation.NavigationGraph
import com.myschool.app.navigation.Screen
import com.myschool.app.ui.screens.dashboard.DashboardScreen
import com.myschool.app.ui.theme.*

@Composable
fun MainApp(
    userProfile: UserProfile,
    onLogout: () -> Unit
) {
    val navController = rememberNavController()
    val navItems = remember(userProfile.roleId) { 
        NavigationGraph.getBottomNavItemsForRole(userProfile.roleId) 
    }
    
    Scaffold(
        bottomBar = {
            NavigationRail(
                modifier = Modifier.fillMaxHeight(),
                containerColor = MaterialTheme.colorScheme.surface,
                contentColor = getRoleColor(userProfile.roleId)
            ) {
                navItems.forEach { item ->
                    NavigationRailItem(
                        selected = false,
                        onClick = {
                            navController.navigate(item.screen.route) {
                                popUpTo(navController.graph.startDestinationId) {
                                    saveState = true
                                }
                                launchSingleTop = true
                                restoreState = true
                            }
                        },
                        icon = {
                            Icon(
                                imageVector = item.screen.icon ?: Icons.Default.Home,
                                contentDescription = item.screen.title
                            )
                        },
                        label = {
                            Text(
                                text = item.screen.title,
                                style = MaterialTheme.typography.labelMedium
                            )
                        },
                        colors = NavigationRailItemDefaults.colors(
                            selectedIconColor = getRoleColor(userProfile.roleId),
                            unselectedIconColor = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f),
                            selectedTextColor = getRoleColor(userProfile.roleId),
                            unselectedTextColor = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f),
                            indicatorColor = getRoleColor(userProfile.roleId).copy(alpha = 0.2f)
                        )
                    )
                }
                
                Spacer(modifier = Modifier.weight(1f))
                
                // Profile and Logout at bottom
                NavigationRailItem(
                    selected = false,
                    onClick = { /* Navigate to profile */ },
                    icon = {
                        Icon(
                            imageVector = Icons.Default.Person,
                            contentDescription = "Profile"
                        )
                    },
                    label = {
                        Text(
                            text = "Profile",
                            style = MaterialTheme.typography.labelMedium
                        )
                    },
                    colors = NavigationRailItemDefaults.colors(
                        selectedIconColor = getRoleColor(userProfile.roleId),
                        unselectedIconColor = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f),
                        selectedTextColor = getRoleColor(userProfile.roleId),
                        unselectedTextColor = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                    )
                )
                
                NavigationRailItem(
                    selected = false,
                    onClick = onLogout,
                    icon = {
                        Icon(
                            imageVector = Icons.Default.ExitToApp,
                            contentDescription = "Logout",
                            tint = ErrorRed
                        )
                    },
                    label = {
                        Text(
                            text = "Logout",
                            style = MaterialTheme.typography.labelMedium,
                            color = ErrorRed
                        )
                    },
                    colors = NavigationRailItemDefaults.colors(
                        selectedIconColor = ErrorRed,
                        unselectedIconColor = ErrorRed,
                        selectedTextColor = ErrorRed,
                        unselectedTextColor = ErrorRed
                    )
                )
            }
        }
    ) { paddingValues ->
        NavHost(
            navController = navController,
            startDestination = Screen.Dashboard.route,
            modifier = Modifier.padding(paddingValues)
        ) {
            composable(Screen.Dashboard.route) {
                DashboardScreen(
                    userProfile = userProfile,
                    onLogout = onLogout
                )
            }
            
            // Add more composables for other screens as they are implemented
            composable(Screen.Students.route) {
                StudentsListScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.AcademicYears.route) {
                AcademicYearsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Terms.route) {
                TermsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Exams.route) {
                ExamsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Subjects.route) {
                SubjectsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Classes.route) {
                ClassesScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Fees.route) {
                FeesScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Payments.route) {
                PaymentsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.Receipts.route) {
                ReceiptsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.GradingSystem.route) {
                GradingSystemScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.ClassTeachers.route) {
                ClassTeachersScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.TeacherAssignments.route) {
                TeacherAssignmentsScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            composable(Screen.StudentPromotion.route) {
                StudentPromotionScreen(
                    userProfile = userProfile,
                    onNavigateBack = { navController.popBackStack() }
                )
            }
        }
    }
}

@Composable
private fun getRoleColor(roleId: Int): Color {
    return when (roleId) {
        1 -> SystemAdminColor
        2 -> DirectorColor
        3 -> HeadmasterColor
        4 -> BursarColor
        5 -> TeacherColor
        else -> PrimaryBlue
    }
}

// Placeholder screens - to be implemented with full functionality
@Composable
private fun StudentsListScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Students", userProfile.roleId)
}

@Composable
private fun AcademicYearsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Academic Years", userProfile.roleId)
}

@Composable
private fun TermsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Terms", userProfile.roleId)
}

@Composable
private fun ExamsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Exams", userProfile.roleId)
}

@Composable
private fun SubjectsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Subjects", userProfile.roleId)
}

@Composable
private fun ClassesScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Classes", userProfile.roleId)
}

@Composable
private fun FeesScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Fees Management", userProfile.roleId)
}

@Composable
private fun PaymentsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Payments", userProfile.roleId)
}

@Composable
private fun ReceiptsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Receipts", userProfile.roleId)
}

@Composable
private fun GradingSystemScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Grading System", userProfile.roleId)
}

@Composable
private fun ClassTeachersScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Class Teachers", userProfile.roleId)
}

@Composable
private fun TeacherAssignmentsScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Teacher Assignments", userProfile.roleId)
}

@Composable
private fun StudentPromotionScreen(userProfile: UserProfile, onNavigateBack: () -> Unit) {
    BoxWithContent("Student Promotion", userProfile.roleId)
}

@Composable
private fun BoxWithContent(title: String, roleId: Int) {
    Box(
        modifier = Modifier
            .fillMaxSize()
            .padding(16.dp),
        contentAlignment = androidx.compose.ui.Alignment.Center
    ) {
        Column(
            horizontalAlignment = androidx.compose.ui.Alignment.CenterHorizontally
        ) {
            Text(
                text = title,
                style = MaterialTheme.typography.headlineMedium,
                color = getRoleColor(roleId)
            )
            Spacer(modifier = Modifier.height(8.dp))
            Text(
                text = "Screen under development",
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
            )
        }
    }
}
