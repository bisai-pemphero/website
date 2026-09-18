package com.myschool.app.ui.screens.login

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschool.app.data.repository.AuthRepository
import com.myschool.app.domain.model.LoginResponse
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class LoginViewModel @Inject constructor(
    private val authRepository: AuthRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<LoginUiState>(LoginUiState.Idle)
    val uiState: StateFlow<LoginUiState> = _uiState

    suspend fun login(username: String, password: String): Result<LoginResponse> {
        _uiState.value = LoginUiState.Loading
        return try {
            val result = authRepository.login(username, password)
            if (result.isSuccess) {
                _uiState.value = LoginUiState.Success(result.getOrNull()!!)
                Result.success(result.getOrNull()!!)
            } else {
                _uiState.value = LoginUiState.Error(result.exceptionOrNull()?.message ?: "Login failed")
                Result.failure(result.exceptionOrNull() ?: Exception("Login failed"))
            }
        } catch (e: Exception) {
            _uiState.value = LoginUiState.Error(e.message ?: "An error occurred")
            Result.failure(e)
        }
    }

    fun resetState() {
        _uiState.value = LoginUiState.Idle
    }
}

sealed class LoginUiState {
    object Idle : LoginUiState()
    object Loading : LoginUiState()
    data class Success(val user: LoginResponse) : LoginUiState()
    data class Error(val message: String) : LoginUiState()
}
