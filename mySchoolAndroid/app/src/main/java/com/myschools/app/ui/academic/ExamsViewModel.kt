package com.myschools.app.ui.academic

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.Exam
import com.myschools.app.data.repository.AcademicRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class ExamsViewModel @Inject constructor(
    private val academicRepository: AcademicRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<ExamsUiState>(ExamsUiState.Loading)
    val uiState: StateFlow<ExamsUiState> = _uiState.asStateFlow()

    init {
        loadExams()
    }

    fun loadExams() {
        viewModelScope.launch {
            _uiState.value = ExamsUiState.Loading
            academicRepository.getExams()
                .onSuccess { exams ->
                    _uiState.value = ExamsUiState.Success(exams)
                }
                .onFailure { error ->
                    _uiState.value = ExamsUiState.Error(error.message ?: "Failed to load exams")
                }
        }
    }

    fun deleteExam(examId: Int) {
        viewModelScope.launch {
            academicRepository.deleteExam(examId)
                .onSuccess {
                    loadExams() // Reload list after deletion
                }
                .onFailure { error ->
                    _uiState.value = ExamsUiState.Error(error.message ?: "Failed to delete exam")
                }
        }
    }
}
