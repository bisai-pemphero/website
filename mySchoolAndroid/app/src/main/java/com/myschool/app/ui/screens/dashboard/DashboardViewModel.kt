package com.myschool.app.ui.screens.dashboard

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschool.app.data.repository.AuthRepository
import com.myschool.app.data.repository.DashboardRepository
import com.myschool.app.domain.model.DashboardStats
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

sealed class DashboardUiState {
    object Idle : DashboardUiState()
    object Loading : DashboardUiState()
    data class Success(val stats: DashboardStats, val userName: String, val roleName: String) : DashboardUiState()
    data class Error(val message: String) : DashboardUiState()
}

@HiltViewModel
class DashboardViewModel @Inject constructor(
    private val dashboardRepository: DashboardRepository,
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<DashboardUiState>(DashboardUiState.Idle)
    val uiState: StateFlow<DashboardUiState> = _uiState.asStateFlow()

    init {
        loadDashboardData()
    }

    private fun loadDashboardData() {
        viewModelScope.launch {
            _uiState.value = DashboardUiState.Loading
            
            // Get current user info from stored data
            val username = authRepository.getUserId() ?: "User"
            val roleId = authRepository.getRoleId() ?: 0
            
            try {
                // Fetch stats from backend
                when (val result = dashboardRepository.getStats()) {
                    is Result.Success -> {
                        val stats = result.getOrNull() ?: DashboardStats()
                        val roleName = getRoleNameFromId(roleId)
                        _uiState.value = DashboardUiState.Success(
                            stats = stats,
                            userName = username,
                            roleName = roleName
                        )
                    }
                    is Result.Failure -> {
                        // Fallback to local user info if API fails
                        val roleName = getRoleNameFromId(roleId)
                        _uiState.value = DashboardUiState.Success(
                            stats = DashboardStats(),
                            userName = username,
                            roleName = roleName
                        )
                    }
                }
            } catch (e: Exception) {
                val roleName = getRoleNameFromId(roleId)
                _uiState.value = DashboardUiState.Success(
                    stats = DashboardStats(),
                    userName = username,
                    roleName = roleName
                )
            }
        }
    }
    
    fun refresh() {
        loadDashboardData()
    }
    
    private fun getRoleNameFromId(roleId: Int): String {
        return when (roleId) {
            1 -> "System Admin"
            2 -> "Director"
            3 -> "Headmaster"
            4 -> "Bursar"
            5 -> "Teacher"
            else -> "User"
        }
    }
}
