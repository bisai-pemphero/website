package com.myschools.app.ui.payments

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.myschools.app.models.PaymentRecord

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun PaymentCollectionScreen(
    students: List<Map<String, Any>> = emptyList(),
    feeCategories: List<Map<String, Any>> = emptyList(),
    onSavePayment: (PaymentData) -> Unit,
    onNavigateBack: () -> Unit,
    isSaving: Boolean = false
) {
    var selectedStudentId by remember { mutableStateOf<Int?>(null) }
    var selectedStudentName by remember { mutableStateOf("") }
    var admissionNo by remember { mutableStateOf("") }
    var amount by remember { mutableStateOf("") }
    var paymentMode by remember { mutableStateOf("Cash") }
    var paidBy by remember { mutableStateOf("") }
    var reference by remember { mutableStateOf("") }
    var selectedFeeCategoryId by remember { mutableStateOf<Int?>(null) }
    
    var studentExpanded by remember { mutableStateOf(false) }
    var paymentModeExpanded by remember { mutableStateOf(false) }
    var feeCategoryExpanded by remember { mutableStateOf(false) }

    val paymentModes = listOf("Cash", "Mobile Money", "Cheque", "Bank Transfer", "Card")
    val studentOptions = students.map { it["studentName"] as String }
    val feeCategoryOptions = feeCategories.map { it["category_name"] as String }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Collect Fee Payment") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        }
    ) { paddingValues ->
        LazyColumn(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            item {
                ExposedDropdownMenuBox(
                    expanded = studentExpanded,
                    onExpandedChange = { studentExpanded = !studentExpanded }
                ) {
                    OutlinedTextField(
                        value = selectedStudentName,
                        onValueChange = {},
                        readOnly = true,
                        label = { Text("Select Student *") },
                        modifier = Modifier.fillMaxWidth().menuAnchor(),
                        trailingIcon = { 
                            if (selectedStudentName.isNotEmpty()) {
                                IconButton(onClick = { 
                                    selectedStudentName = ""
                                    selectedStudentId = null
                                    admissionNo = ""
                                }) {
                                    Icon(Icons.Default.Clear, contentDescription = "Clear")
                                }
                            } else {
                                ExposedDropdownMenuDefaults.TrailingIcon(expanded = studentExpanded)
                            }
                        },
                        isError = selectedStudentId == null,
                        supportingText = if (selectedStudentId == null) { { Text("Required") } } else null
                    )
                    ExposedDropdownMenu(
                        expanded = studentExpanded,
                        onDismissRequest = { studentExpanded = false }
                    ) {
                        studentOptions.forEach { option ->
                            DropdownMenuItem(
                                text = { Text(option) },
                                onClick = {
                                    val student = students.find { it["studentName"] == option }
                                    selectedStudentName = option
                                    selectedStudentId = student?.get("studentID") as? Int
                                    admissionNo = student?.get("admissionNo") as? String ?: ""
                                    studentExpanded = false
                                }
                            )
                        }
                    }
                }
            }

            item {
                OutlinedTextField(
                    value = admissionNo,
                    onValueChange = {},
                    readOnly = true,
                    label = { Text("Admission Number") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true,
                    enabled = false
                )
            }

            item {
                ExposedDropdownMenuBox(
                    expanded = feeCategoryExpanded,
                    onExpandedChange = { feeCategoryExpanded = !feeCategoryExpanded }
                ) {
                    OutlinedTextField(
                        value = feeCategoryOptions.find { 
                            feeCategories.find { c -> c["category_name"] == it }?.let { c -> 
                                c["categoryId"] == selectedFeeCategoryId 
                            } ?: false 
                        } ?: "",
                        onValueChange = {},
                        readOnly = true,
                        label = { Text("Fee Category *") },
                        modifier = Modifier.fillMaxWidth().menuAnchor(),
                        trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = feeCategoryExpanded) },
                        isError = selectedFeeCategoryId == null,
                        supportingText = if (selectedFeeCategoryId == null) { { Text("Required") } } else null
                    )
                    ExposedDropdownMenu(
                        expanded = feeCategoryExpanded,
                        onDismissRequest = { feeCategoryExpanded = false }
                    ) {
                        feeCategoryOptions.forEach { option ->
                            DropdownMenuItem(
                                text = { Text(option) },
                                onClick = {
                                    val category = feeCategories.find { it["category_name"] == option }
                                    selectedFeeCategoryId = category?.get("categoryId") as? Int
                                    feeCategoryExpanded = false
                                }
                            )
                        }
                    }
                }
            }

            item {
                OutlinedTextField(
                    value = amount,
                    onValueChange = { if (it.all { char -> char.isDigit() || char == '.' }) amount = it },
                    label = { Text("Amount *") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true,
                    leadingIcon = { Text("GHS ", fontWeight = FontWeight.Bold) },
                    isError = amount.isBlank() || amount.toDoubleOrNull() == null || amount.toDouble() <= 0,
                    supportingText = { 
                        if (amount.isBlank()) {
                            Text("Required")
                        } else if (amount.toDoubleOrNull() == null || amount.toDouble() <= 0) {
                            Text("Must be greater than 0")
                        }
                    }
                )
            }

            item {
                ExposedDropdownMenuBox(
                    expanded = paymentModeExpanded,
                    onExpandedChange = { paymentModeExpanded = !paymentModeExpanded }
                ) {
                    OutlinedTextField(
                        value = paymentMode,
                        onValueChange = {},
                        readOnly = true,
                        label = { Text("Payment Mode *") },
                        modifier = Modifier.fillMaxWidth().menuAnchor(),
                        trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(expanded = paymentModeExpanded) }
                    )
                    ExposedDropdownMenu(
                        expanded = paymentModeExpanded,
                        onDismissRequest = { paymentModeExpanded = false }
                    ) {
                        paymentModes.forEach { option ->
                            DropdownMenuItem(
                                text = { Text(option) },
                                onClick = {
                                    paymentMode = option
                                    paymentModeExpanded = false
                                }
                            )
                        }
                    }
                }
            }

            item {
                OutlinedTextField(
                    value = paidBy,
                    onValueChange = { paidBy = it },
                    label = { Text("Paid By (Payer Name)") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true,
                    placeholder = { Text("e.g., John Doe (Parent)") }
                )
            }

            item {
                OutlinedTextField(
                    value = reference,
                    onValueChange = { reference = it },
                    label = { Text("Payment Reference") },
                    modifier = Modifier.fillMaxWidth(),
                    singleLine = true,
                    placeholder = { Text("e.g., Cheque No. or Transaction ID") }
                )
            }

            item {
                Spacer(modifier = Modifier.height(16.dp))
            }

            item {
                Button(
                    onClick = {
                        val paymentData = PaymentData(
                            feesId = 0,
                            studentId = selectedStudentId ?: 0,
                            amount = amount.toDoubleOrNull() ?: 0.0,
                            paymentMode = paymentMode,
                            paidBy = paidBy.ifBlank { null },
                            paymentReference = reference.ifBlank { null }
                        )
                        onSavePayment(paymentData)
                    },
                    modifier = Modifier.fillMaxWidth(),
                    enabled = !isSaving && 
                              selectedStudentId != null && 
                              selectedFeeCategoryId != null &&
                              amount.isNotBlank() && 
                              amount.toDoubleOrNull() != null && 
                              amount.toDouble() > 0
                ) {
                    if (isSaving) {
                        CircularProgressIndicator(
                            modifier = Modifier.size(24.dp),
                            color = MaterialTheme.colorScheme.onPrimary
                        )
                        Spacer(modifier = Modifier.width(8.dp))
                        Text("Processing...")
                    } else {
                        Icon(Icons.Default.Payment, contentDescription = null)
                        Spacer(modifier = Modifier.width(8.dp))
                        Text("Process Payment")
                    }
                }
            }

            item {
                Card(
                    modifier = Modifier.fillMaxWidth(),
                    colors = CardDefaults.cardColors(
                        containerColor = MaterialTheme.colorScheme.primaryContainer
                    )
                ) {
                    Column(
                        modifier = Modifier.padding(16.dp),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        Text(
                            "Payment Information",
                            style = MaterialTheme.typography.titleMedium,
                            fontWeight = FontWeight.Bold
                        )
                        Text(
                            "• Receipt will be generated automatically",
                            style = MaterialTheme.typography.bodySmall
                        )
                        Text(
                            "• Ensure all details are correct before submitting",
                            style = MaterialTheme.typography.bodySmall
                        )
                        Text(
                            "• Receipt can be reprinted from the Receipts screen",
                            style = MaterialTheme.typography.bodySmall
                        )
                    }
                }
            }
        }
    }
}

data class PaymentData(
    val feesId: Int,
    val studentId: Int,
    val amount: Double,
    val paymentMode: String,
    val paidBy: String?,
    val paymentReference: String?
)
