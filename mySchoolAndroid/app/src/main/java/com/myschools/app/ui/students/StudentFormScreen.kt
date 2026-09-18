package com.myschools.app.ui.students

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import com.myschools.app.models.Student

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun StudentFormScreen(
    student: Student? = null,
    classes: List<Map<String, Any>> = emptyList(),
    onSaveClick: (Student) -> Unit,
    onCancelClick: () -> Unit,
    isSaving: Boolean = false
) {
    var admissionNo by remember { mutableStateOf(student?.admissionNo ?: "") }
    var firstName by remember { mutableStateOf(student?.firstName ?: "") }
    var middleName by remember { mutableStateOf(student?.middleName ?: "") }
    var lastName by remember { mutableStateOf(student?.lastName ?: "") }
    var gender by remember { mutableStateOf(student?.gender ?: "") }
    var dateOfBirth by remember { mutableStateOf(student?.dateOfBirth ?: "") }
    var currentClassID by remember { mutableStateOf(student?.currentClassID ?: 0) }
    var previousSchool by remember { mutableStateOf(student?.prevSchool ?: "") }
    var specialNeeds by remember { mutableStateOf(student?.specialNeeds ?: "") }

    val genders = listOf("Male", "Female")
    val classOptions = classes.map { it["className"] as String }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(if (student == null) "Add Student" else "Edit Student") },
                navigationIcon = {
                    IconButton(onClick = onCancelClick) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    IconButton(
                        onClick = {
                            val newStudent = Student(
                                studentID = student?.studentID ?: 0,
                                admissionNo = admissionNo,
                                firstName = firstName,
                                middleName = middleName.ifBlank { null },
                                lastName = lastName,
                                gender = gender.ifBlank { null },
                                dateOfBirth = dateOfBirth.ifBlank { null },
                                currentClassID = if (currentClassID > 0) currentClassID else null,
                                prevSchool = previousSchool.ifBlank { null },
                                specialNeeds = specialNeeds.ifBlank { null }
                            )
                            onSaveClick(newStudent)
                        },
                        enabled = !isSaving && admissionNo.isNotBlank() && firstName.isNotBlank() && lastName.isNotBlank()
                    ) {
                        Icon(Icons.Default.Check, contentDescription = "Save")
                    }
                }
            )
        }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            // Admission Number
            OutlinedTextField(
                value = admissionNo,
                onValueChange = { admissionNo = it },
                label = { Text("Admission Number *") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
                isError = admissionNo.isBlank(),
                supportingText = if (admissionNo.isBlank()) { { Text("Required") } } else null
            )

            // First Name
            OutlinedTextField(
                value = firstName,
                onValueChange = { firstName = it },
                label = { Text("First Name *") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
                isError = firstName.isBlank(),
                supportingText = if (firstName.isBlank()) { { Text("Required") } } else null
            )

            // Middle Name
            OutlinedTextField(
                value = middleName,
                onValueChange = { middleName = it },
                label = { Text("Middle Name") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true
            )

            // Last Name
            OutlinedTextField(
                value = lastName,
                onValueChange = { lastName = it },
                label = { Text("Last Name *") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
                isError = lastName.isBlank(),
                supportingText = if (lastName.isBlank()) { { Text("Required") } } else null
            )

            // Gender
            var genderExpanded by remember { mutableStateOf(false) }
            ExposedDropdownMenuBox(
                expanded = genderExpanded,
                onExpandedChange = { genderExpanded = !genderExpanded }
            ) {
                OutlinedTextField(
                    value = gender,
                    onValueChange = {},
                    readOnly = true,
                    label = { Text("Gender") },
                    modifier = Modifier
                        .fillMaxWidth()
                        .menuAnchor(),
                    trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = genderExpanded) }
                )
                ExposedDropdownMenu(
                    expanded = genderExpanded,
                    onDismissRequest = { genderExpanded = false }
                ) {
                    genders.forEach { option ->
                        DropdownMenuItem(
                            text = { Text(option) },
                            onClick = {
                                gender = option
                                genderExpanded = false
                            }
                        )
                    }
                }
            }

            // Date of Birth
            OutlinedTextField(
                value = dateOfBirth,
                onValueChange = { dateOfBirth = it },
                label = { Text("Date of Birth (YYYY-MM-DD)") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
                placeholder = { Text("e.g., 2010-05-15") }
            )

            // Class
            var classExpanded by remember { mutableStateOf(false) }
            ExposedDropdownMenuBox(
                expanded = classExpanded,
                onExpandedChange = { classExpanded = !classExpanded }
            ) {
                OutlinedTextField(
                    value = classOptions.find { 
                        classes.find { c -> c["className"] == it }?.let { c -> 
                            c["classID"] == currentClassID 
                        } ?: false 
                    } ?: "",
                    onValueChange = {},
                    readOnly = true,
                    label = { Text("Class") },
                    modifier = Modifier
                        .fillMaxWidth()
                        .menuAnchor(),
                    trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = classExpanded) }
                )
                ExposedDropdownMenu(
                    expanded = classExpanded,
                    onDismissRequest = { classExpanded = false }
                ) {
                    classOptions.forEach { option ->
                        DropdownMenuItem(
                            text = { Text(option) },
                            onClick = {
                                currentClassID = classes.find { it["className"] == option }?.get("classID") as? Int ?: 0
                                classExpanded = false
                            }
                        )
                    }
                }
            }

            // Previous School
            OutlinedTextField(
                value = previousSchool,
                onValueChange = { previousSchool = it },
                label = { Text("Previous School") },
                modifier = Modifier.fillMaxWidth(),
                singleLine = true
            )

            // Special Needs
            OutlinedTextField(
                value = specialNeeds,
                onValueChange = { specialNeeds = it },
                label = { Text("Special Needs") },
                modifier = Modifier.fillMaxWidth(),
                minLines = 3,
                maxLines = 5
            )

            Spacer(modifier = Modifier.weight(1f))

            // Save Button
            Button(
                onClick = {
                    val newStudent = Student(
                        studentID = student?.studentID ?: 0,
                        admissionNo = admissionNo,
                        firstName = firstName,
                        middleName = middleName.ifBlank { null },
                        lastName = lastName,
                        gender = gender.ifBlank { null },
                        dateOfBirth = dateOfBirth.ifBlank { null },
                        currentClassID = if (currentClassID > 0) currentClassID else null,
                        prevSchool = previousSchool.ifBlank { null },
                        specialNeeds = specialNeeds.ifBlank { null }
                    )
                    onSaveClick(newStudent)
                },
                modifier = Modifier.fillMaxWidth(),
                enabled = !isSaving && admissionNo.isNotBlank() && firstName.isNotBlank() && lastName.isNotBlank()
            ) {
                if (isSaving) {
                    CircularProgressIndicator(
                        modifier = Modifier.size(24.dp),
                        color = MaterialTheme.colorScheme.onPrimary
                    )
                    Spacer(modifier = Modifier.width(8.dp))
                    Text("Saving...")
                } else {
                    Icon(Icons.Default.Save, contentDescription = null)
                    Spacer(modifier = Modifier.width(8.dp))
                    Text("Save Student")
                }
            }
        }
    }
}
