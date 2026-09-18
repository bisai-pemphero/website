package com.myschools.app.ui.academic

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.Term
import com.myschools.app.data.repository.AcademicRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class TermsViewModel @Inject constructor(
    private val academicRepository: AcademicRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<TermsUiState>(TermsUiState.Loading)
    val uiState: StateFlow<TermsUiState> = _uiState.asStateFlow()

    init {
        loadTerms()
    }

    fun loadTerms() {
        viewModelScope.launch {
            _uiState.value = TermsUiState.Loading
            academicRepository.getTerms()
                .onSuccess { terms ->
                    _uiState.value = TermsUiState.Success(terms)
                }
                .onFailure { error ->
                    _uiState.value = TermsUiState.Error(error.message ?: "Failed to load terms")
                }
        }
    }

    fun deleteTerm(termId: Int) {
        viewModelScope.launch {
            academicRepository.deleteTerm(termId)
                .onSuccess {
                    loadTerms() // Reload list after deletion
                }
                .onFailure { error ->
                    _uiState.value = TermsUiState.Error(error.message ?: "Failed to delete term")
                }
        }
    }
}
