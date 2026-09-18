package com.myschools.app.ui.academic

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.myschool.app.data.repository.AcademicRepository
import com.myschool.app.domain.model.AcademicYear
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

data class AcademicYearUiState(
    val isLoading: Boolean = false,
    val academicYears: List<AcademicYear> = emptyList(),
    val error: String? = null,
    val showDialog: Boolean = false,
    val editingYear: AcademicYear? = null,
    val yearName: String = "",
    val startDate: String = "",
    val endDate: String = "",
    val isActive: Boolean = false
)

@HiltViewModel
class AcademicYearsViewModel @Inject constructor(
    private val academicRepository: AcademicRepository
) : ViewModel() {
    
    private val _uiState = MutableStateFlow(AcademicYearUiState())
    val uiState: StateFlow<AcademicYearUiState> = _uiState.asStateFlow()
    
    init {
        loadAcademicYears()
    }
    
    fun loadAcademicYears() {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, error = null) }
            academicRepository.getAcademicYears().fold(
                onSuccess = { years ->
                    _uiState.update { 
                        it.copy(
                            isLoading = false,
                            academicYears = years,
                            error = null
                        )
                    }
                },
                onFailure = { error ->
                    _uiState.update { 
                        it.copy(
                            isLoading = false,
                            error = error.message ?: "Failed to load academic years"
                        )
                    }
                }
            )
        }
    }
    
    fun showCreateDialog() {
        _uiState.update {
            it.copy(
                showDialog = true,
                editingYear = null,
                yearName = "",
                startDate = "",
                endDate = "",
                isActive = false
            )
        }
    }
    
    fun showEditDialog(year: AcademicYear) {
        _uiState.update {
            it.copy(
                showDialog = true,
                editingYear = year,
                yearName = year.yearName,
                startDate = year.startDate,
                endDate = year.endDate,
                isActive = year.isActive
            )
        }
    }
    
    fun dismissDialog() {
        _uiState.update { it.copy(showDialog = false, editingYear = null) }
    }
    
    fun updateFormFields(
        yearName: String? = null,
        startDate: String? = null,
        endDate: String? = null,
        isActive: Boolean? = null
    ) {
        _uiState.update { current ->
            current.copy(
                yearName = yearName ?: current.yearName,
                startDate = startDate ?: current.startDate,
                endDate = endDate ?: current.endDate,
                isActive = isActive ?: current.isActive
            )
        }
    }
    
    fun saveAcademicYear(schoolId: Int) {
        viewModelScope.launch {
            val state = _uiState.value
            val year = AcademicYear(
                academicYearId = state.editingYear?.academicYearId ?: 0,
                yearName = state.yearName,
                startDate = state.startDate,
                endDate = state.endDate,
                isActive = state.isActive,
                schoolId = schoolId
            )
            
            _uiState.update { it.copy(isLoading = true, error = null) }
            
            val result = if (state.editingYear != null) {
                academicRepository.updateAcademicYear(year.academicYearId, year)
            } else {
                academicRepository.createAcademicYear(year)
            }
            
            result.fold(
                onSuccess = {
                    _uiState.update { 
                        it.copy(
                            isLoading = false,
                            showDialog = false,
                            editingYear = null,
                            yearName = "",
                            startDate = "",
                            endDate = ""
                        )
                    }
                    loadAcademicYears()
                },
                onFailure = { error ->
                    _uiState.update { 
                        it.copy(
                            isLoading = false,
                            error = error.message ?: "Failed to save academic year"
                        )
                    }
                }
            )
        }
    }
    
    fun deleteAcademicYear(yearId: Int) {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, error = null) }
            academicRepository.deleteAcademicYear(yearId).fold(
                onSuccess = {
                    loadAcademicYears()
                },
                onFailure = { error ->
                    _uiState.update { 
                        it.copy(
                            isLoading = false,
                            error = error.message ?: "Failed to delete academic year"
                        )
                    }
                }
            )
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun AcademicYearsScreen(
    viewModel: AcademicYearsViewModel = hiltViewModel(),
    schoolId: Int,
    onNavigateBack: () -> Unit = {}
) {
    val uiState by viewModel.uiState.collectAsStateWithLifecycle()
    var showDeleteConfirmation by remember { mutableStateOf(false) }
    var yearToDelete by remember { mutableStateOf<Int?>(null) }
    
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Academic Years") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    IconButton(onClick = { viewModel.loadAcademicYears() }) {
                        Icon(Icons.Default.Refresh, contentDescription = "Refresh")
                    }
                },
                colors = TopAppBarDefaults.topAppBarColors(
                    containerColor = MaterialTheme.colorScheme.primaryContainer,
                    titleContentColor = MaterialTheme.colorScheme.onPrimaryContainer
                )
            )
        },
        floatingActionButton = {
            FloatingActionButton(
                onClick = { viewModel.showCreateDialog() },
                containerColor = MaterialTheme.colorScheme.primary
            ) {
                Icon(Icons.Default.Add, contentDescription = "Add Academic Year")
            }
        }
    ) { paddingValues ->
        Box(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            when {
                uiState.isLoading && uiState.academicYears.isEmpty() -> {
                    CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.Center),
                        color = MaterialTheme.colorScheme.primary
                    )
                }
                
                uiState.error != null && uiState.academicYears.isEmpty() -> {
                    Column(
                        modifier = Modifier.align(Alignment.Center),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Icon(
                            Icons.Default.ErrorOutline,
                            contentDescription = null,
                            tint = MaterialTheme.colorScheme.error,
                            modifier = Modifier.size(64.dp)
                        )
                        Spacer(modifier = Modifier.height(16.dp))
                        Text(
                            text = uiState.error!!,
                            style = MaterialTheme.typography.bodyLarge,
                            color = MaterialTheme.colorScheme.error
                        )
                        Spacer(modifier = Modifier.height(16.dp))
                        Button(onClick = { viewModel.loadAcademicYears() }) {
                            Text("Retry")
                        }
                    }
                }
                
                uiState.academicYears.isEmpty() -> {
                    Column(
                        modifier = Modifier.align(Alignment.Center),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Icon(
                            Icons.Default.CalendarMonth,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                        Spacer(modifier = Modifier.height(16.dp))
                        Text(
                            text = "No academic years found",
                            style = MaterialTheme.typography.bodyLarge,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f)
                        )
                        Spacer(modifier = Modifier.height(8.dp))
                        Text(
                            text = "Tap + to add your first academic year",
                            style = MaterialTheme.typography.bodyMedium,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                }
                
                else -> {
                    LazyColumn(
                        modifier = Modifier.fillMaxSize(),
                        contentPadding = PaddingValues(16.dp),
                        verticalArrangement = Arrangement.spacedBy(12.dp)
                    ) {
                        items(uiState.academicYears, key = { it.academicYearId }) { year ->
                            AcademicYearCard(
                                academicYear = year,
                                onEdit = { viewModel.showEditDialog(year) },
                                onDelete = {
                                    yearToDelete = year.academicYearId
                                    showDeleteConfirmation = true
                                }
                            )
                        }
                    }
                }
            }
            
            // Loading overlay for operations
            if (uiState.isLoading && !uiState.academicYears.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .background(MaterialTheme.colorScheme.surface.copy(alpha = 0.7f)),
                    contentAlignment = Alignment.Center
                ) {
                    CircularProgressIndicator(
                        color = MaterialTheme.colorScheme.primary
                    )
                }
            }
        }
        
        // Delete confirmation dialog
        if (showDeleteConfirmation && yearToDelete != null) {
            AlertDialog(
                onDismissRequest = {
                    showDeleteConfirmation = false
                    yearToDelete = null
                },
                icon = {
                    Icon(
                        Icons.Default.Warning,
                        contentDescription = null,
                        tint = MaterialTheme.colorScheme.error
                    )
                },
                title = { Text("Delete Academic Year") },
                text = { Text("Are you sure you want to delete this academic year? This action cannot be undone.") },
                confirmButton = {
                    TextButton(
                        onClick = {
                            viewModel.deleteAcademicYear(yearToDelete!!)
                            showDeleteConfirmation = false
                            yearToDelete = null
                        },
                        colors = ButtonDefaults.textButtonColors(
                            contentColor = MaterialTheme.colorScheme.error
                        )
                    ) {
                        Text("Delete")
                    }
                },
                dismissButton = {
                    TextButton(onClick = {
                        showDeleteConfirmation = false
                        yearToDelete = null
                    }) {
                        Text("Cancel")
                    }
                }
            )
        }
        
        // Create/Edit dialog
        if (uiState.showDialog) {
            AcademicYearDialog(
                uiState = uiState,
                onDismiss = { viewModel.dismissDialog() },
                onSave = { viewModel.saveAcademicYear(schoolId) },
                onFieldChange = { name, start, end, active ->
                    viewModel.updateFormFields(name, start, end, active)
                }
            )
        }
    }
}

@Composable
private fun AcademicYearCard(
    academicYear: AcademicYear,
    onEdit: () -> Unit,
    onDelete: () -> Unit
) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp),
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surfaceVariant
        )
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp)
        ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Column(modifier = Modifier.weight(1f)) {
                    Text(
                        text = academicYear.yearName,
                        style = MaterialTheme.typography.titleMedium,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(
                        text = "${academicYear.startDate} - ${academicYear.endDate}",
                        style = MaterialTheme.typography.bodyMedium,
                        color = MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.7f)
                    )
                }
                
                if (academicYear.isActive) {
                    AssistChip(
                        onClick = { },
                        label = {
                            Text(
                                text = "Active",
                                style = MaterialTheme.typography.labelSmall
                            )
                        },
                        leadingIcon = {
                            Icon(
                                Icons.Default.CheckCircle,
                                contentDescription = null,
                                modifier = Modifier.size(16.dp)
                            )
                        },
                        colors = AssistChipDefaults.assistChipColors(
                            containerColor = MaterialTheme.colorScheme.primaryContainer
                        )
                    )
                }
            }
            
            Spacer(modifier = Modifier.height(12.dp))
            
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.End
            ) {
                TextButton(onClick = onEdit) {
                    Icon(
                        Icons.Default.Edit,
                        contentDescription = null,
                        modifier = Modifier.size(18.dp)
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text("Edit")
                }
                Spacer(modifier = Modifier.width(8.dp))
                TextButton(
                    onClick = onDelete,
                    colors = ButtonDefaults.textButtonColors(
                        contentColor = MaterialTheme.colorScheme.error
                    )
                ) {
                    Icon(
                        Icons.Default.Delete,
                        contentDescription = null,
                        modifier = Modifier.size(18.dp)
                    )
                    Spacer(modifier = Modifier.width(4.dp))
                    Text("Delete")
                }
            }
        }
    }
}

@Composable
private fun AcademicYearDialog(
    uiState: AcademicYearUiState,
    onDismiss: () -> Unit,
    onSave: () -> Unit,
    onFieldChange: (String?, String?, String?, Boolean?) -> Unit
) {
    AlertDialog(
        onDismissRequest = onDismiss,
        title = {
            Text(
                if (uiState.editingYear != null) "Edit Academic Year" else "Add Academic Year"
            )
        },
        text = {
            Column(
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                OutlinedTextField(
                    value = uiState.yearName,
                    onValueChange = { onFieldChange(it, null, null, null) },
                    label = { Text("Year Name *") },
                    placeholder = { Text("e.g., 2024/2025") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true,
                    isError = uiState.yearName.isBlank()
                )
                
                OutlinedTextField(
                    value = uiState.startDate,
                    onValueChange = { onFieldChange(null, it, null, null) },
                    label = { Text("Start Date *") },
                    placeholder = { Text("e.g., 2024-09-01") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true
                )
                
                OutlinedTextField(
                    value = uiState.endDate,
                    onValueChange = { onFieldChange(null, null, it, null) },
                    label = { Text("End Date *") },
                    placeholder = { Text("e.g., 2025-07-31") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true
                )
                
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(
                        text = "Set as Active Year",
                        style = MaterialTheme.typography.bodyMedium
                    )
                    Switch(
                        checked = uiState.isActive,
                        onCheckedChange = { onFieldChange(null, null, null, it) }
                    )
                }
            }
        },
        confirmButton = {
            Button(
                onClick = onSave,
                enabled = uiState.yearName.isNotBlank() && 
                          uiState.startDate.isNotBlank() && 
                          uiState.endDate.isNotBlank()
            ) {
                Text(if (uiState.editingYear != null) "Update" else "Create")
            }
        },
        dismissButton = {
            TextButton(onClick = onDismiss) {
                Text("Cancel")
            }
        }
    )
}
