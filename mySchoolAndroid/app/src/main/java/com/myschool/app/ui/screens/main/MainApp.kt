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
                    val currentRoute = navController.currentBackStackEntryAsState().value?.destination?.route
                    val isSelected = currentRoute == item.screen.route
                    
                    NavigationRailItem(
                        selected = isSelected,
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
                    onLogout = onLogout,
                    onNavigateToScreen = { route ->
                        navController.navigate(route) {
                            popUpTo(Screen.Dashboard.route) {
                                saveState = true
                            }
                            launchSingleTop = true
                            restoreState = true
                        }
                    }
                )
            }
            
            // Students screen - fully implemented
            composable(Screen.Students.route) {
                StudentsListScreen(
                    onStudentClick = { /* Navigate to student details */ },
                    onCreateStudent = { /* Navigate to create student form */ },
                    onNavigateBack = { navController.popBackStack() }
                )
            }
            
            // Placeholder screens - to be implemented with full functionality
            composable(Screen.AcademicYears.route) {
                BoxWithContent("Academic Years", userProfile.roleId)
            }
            
            composable(Screen.Terms.route) {
                BoxWithContent("Terms", userProfile.roleId)
            }
            
            composable(Screen.Exams.route) {
                BoxWithContent("Exams", userProfile.roleId)
            }
            
            composable(Screen.Subjects.route) {
                BoxWithContent("Subjects", userProfile.roleId)
            }
            
            composable(Screen.Classes.route) {
                BoxWithContent("Classes", userProfile.roleId)
            }
            
            composable(Screen.Fees.route) {
                BoxWithContent("Fees Management", userProfile.roleId)
            }
            
            composable(Screen.Payments.route) {
                BoxWithContent("Payments", userProfile.roleId)
            }
            
            composable(Screen.Receipts.route) {
                BoxWithContent("Receipts", userProfile.roleId)
            }
            
            composable(Screen.GradingSystem.route) {
                BoxWithContent("Grading System", userProfile.roleId)
            }
            
            composable(Screen.ClassTeachers.route) {
                BoxWithContent("Class Teachers", userProfile.roleId)
            }
            
            composable(Screen.TeacherAssignments.route) {
                BoxWithContent("Teacher Assignments", userProfile.roleId)
            }
            
            composable(Screen.StudentPromotion.route) {
                BoxWithContent("Student Promotion", userProfile.roleId)
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
