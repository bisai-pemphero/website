package com.myschools.app.ui.academic

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.GradeRequest
import com.myschools.app.data.repository.GradingRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class GradingSystemViewModel @Inject constructor(
    private val gradingRepository: GradingRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<GradingSystemUiState>(GradingSystemUiState.Loading)
    val uiState: StateFlow<GradingSystemUiState> = _uiState.asStateFlow()

    init {
        loadGradingSystems()
    }

    fun loadGradingSystems() {
        viewModelScope.launch {
            _uiState.value = GradingSystemUiState.Loading
            gradingRepository.getGradingSystems()
                .onSuccess { grades ->
                    _uiState.value = GradingSystemUiState.Success(grades)
                }
                .onFailure { error ->
                    _uiState.value = GradingSystemUiState.Error(error.message ?: "Failed to load grading system")
                }
        }
    }

    fun deleteGradingSystem(gradeId: Int) {
        viewModelScope.launch {
            gradingRepository.deleteGradingSystem(gradeId)
                .onSuccess {
                    loadGradingSystems() // Reload list after deletion
                }
                .onFailure { error ->
                    _uiState.value = GradingSystemUiState.Error(error.message ?: "Failed to delete grade")
                }
        }
    }
}
