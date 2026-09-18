package com.myschools.app.ui.admin

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.AllSchool
import com.myschools.app.data.repository.SchoolsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class SchoolsViewModel @Inject constructor(
    private val schoolsRepository: SchoolsRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<SchoolsUiState>(SchoolsUiState.Loading)
    val uiState: StateFlow<SchoolsUiState> = _uiState.asStateFlow()

    init {
        loadSchools()
    }

    fun loadSchools() {
        viewModelScope.launch {
            _uiState.value = SchoolsUiState.Loading
            schoolsRepository.getAllSchools()
                .onSuccess { schools ->
                    _uiState.value = SchoolsUiState.Success(schools)
                }
                .onFailure { error ->
                    _uiState.value = SchoolsUiState.Error(error.message ?: "Failed to load schools")
                }
        }
    }
}
