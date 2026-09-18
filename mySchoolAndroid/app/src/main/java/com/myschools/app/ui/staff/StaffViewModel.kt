package com.myschools.app.ui.staff

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.User
import com.myschools.app.data.repository.UsersRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class StaffViewModel @Inject constructor(
    private val usersRepository: UsersRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<StaffUiState>(StaffUiState.Loading)
    val uiState: StateFlow<StaffUiState> = _uiState.asStateFlow()

    init {
        loadStaff()
    }

    fun loadStaff() {
        viewModelScope.launch {
            _uiState.value = StaffUiState.Loading
            usersRepository.getUsers()
                .onSuccess { users ->
                    _uiState.value = StaffUiState.Success(users)
                }
                .onFailure { error ->
                    _uiState.value = StaffUiState.Error(error.message ?: "Failed to load staff")
                }
        }
    }
}
