package com.myschool.app.ui.screens.students

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschool.app.data.repository.StudentRepository
import com.myschool.app.domain.model.Student
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class StudentsViewModel @Inject constructor(
    private val studentRepository: StudentRepository
) : ViewModel() {
    
    private val _uiState = MutableStateFlow<StudentsUiState>(StudentsUiState.Loading)
    val uiState: StateFlow<StudentsUiState> = _uiState
    
    init {
        loadStudents()
    }
    
    fun loadStudents() {
        viewModelScope.launch {
            _uiState.value = StudentsUiState.Loading
            studentRepository.getStudents().fold(
                onSuccess = { students ->
                    _uiState.value = StudentsUiState.Success(students)
                },
                onFailure = { error ->
                    _uiState.value = StudentsUiState.Error(error.message ?: "Unknown error")
                }
            )
        }
    }
}
