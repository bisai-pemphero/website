package com.myschools.app.ui.academic

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.Subject
import com.myschools.app.data.repository.AcademicRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class SubjectsViewModel @Inject constructor(
    private val academicRepository: AcademicRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<SubjectsUiState>(SubjectsUiState.Loading)
    val uiState: StateFlow<SubjectsUiState> = _uiState.asStateFlow()

    init {
        loadSubjects()
    }

    fun loadSubjects() {
        viewModelScope.launch {
            _uiState.value = SubjectsUiState.Loading
            academicRepository.getSubjects()
                .onSuccess { subjects ->
                    _uiState.value = SubjectsUiState.Success(subjects)
                }
                .onFailure { error ->
                    _uiState.value = SubjectsUiState.Error(error.message ?: "Failed to load subjects")
                }
        }
    }

    fun deleteSubject(subjectId: Int) {
        viewModelScope.launch {
            academicRepository.deleteSubject(subjectId)
                .onSuccess {
                    loadSubjects() // Reload list after deletion
                }
                .onFailure { error ->
                    _uiState.value = SubjectsUiState.Error(error.message ?: "Failed to delete subject")
                }
        }
    }
}
