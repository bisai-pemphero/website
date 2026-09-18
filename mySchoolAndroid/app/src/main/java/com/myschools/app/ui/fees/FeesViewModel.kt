package com.myschools.app.ui.fees

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschools.app.data.model.FeesCategory
import com.myschools.app.data.repository.FeesRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class FeesViewModel @Inject constructor(
    private val feesRepository: FeesRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow<FeesUiState>(FeesUiState.Loading)
    val uiState: StateFlow<FeesUiState> = _uiState.asStateFlow()

    init {
        loadFeesCategories()
    }

    fun loadFeesCategories() {
        viewModelScope.launch {
            _uiState.value = FeesUiState.Loading
            feesRepository.getFeesCategories()
                .onSuccess { categories ->
                    _uiState.value = FeesUiState.Success(categories)
                }
                .onFailure { error ->
                    _uiState.value = FeesUiState.Error(error.message ?: "Failed to load fee categories")
                }
        }
    }

    fun deleteFeesCategory(categoryId: Int) {
        viewModelScope.launch {
            feesRepository.deleteFeesCategory(categoryId)
                .onSuccess {
                    loadFeesCategories() // Reload list after deletion
                }
                .onFailure { error ->
                    _uiState.value = FeesUiState.Error(error.message ?: "Failed to delete fee category")
                }
        }
    }
}
