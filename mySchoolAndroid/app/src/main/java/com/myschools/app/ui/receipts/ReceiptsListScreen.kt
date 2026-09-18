package com.myschools.app.ui.receipts

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.myschools.app.models.PaymentRecord

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ReceiptsListScreen(
    payments: List<PaymentRecord>,
    isLoading: Boolean = false,
    onRefresh: () -> Unit,
    onReceiptClick: (PaymentRecord) -> Unit,
    onNavigateBack: () -> Unit,
    searchQuery: String = "",
    onSearchQueryChange: (String) -> Unit
) {
    var showSearch by remember { mutableStateOf(false) }
    
    val filteredPayments = remember(payments, searchQuery) {
        if (searchQuery.isBlank()) {
            payments
        } else {
            payments.filter { 
                it.receipNumber?.contains(searchQuery, ignoreCase = true) == true ||
                it.paidBy?.contains(searchQuery, ignoreCase = true) == true ||
                it.studentName?.contains(searchQuery, ignoreCase = true) == true
            }
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Receipts") },
                navigationIcon = {
                    IconButton(onClick = onNavigateBack) {
                        Icon(Icons.Default.ArrowBack, contentDescription = "Back")
                    }
                },
                actions = {
                    IconButton(onClick = { showSearch = !showSearch }) {
                        Icon(
                            if (showSearch) Icons.Default.Close else Icons.Default.Search,
                            contentDescription = if (showSearch) "Close Search" else "Search"
                        )
                    }
                }
            )
        }
    ) { paddingValues ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(paddingValues)
        ) {
            if (showSearch) {
                OutlinedTextField(
                    value = searchQuery,
                    onValueChange = onSearchQueryChange,
                    placeholder = { Text("Search by receipt no., student, or payer") },
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = 16.dp, vertical = 8.dp),
                    singleLine = true,
                    leadingIcon = { Icon(Icons.Default.Search, contentDescription = null) },
                    trailingIcon = {
                        if (searchQuery.isNotEmpty()) {
                            IconButton(onClick = { onSearchQueryChange("") }) {
                                Icon(Icons.Default.Clear, contentDescription = "Clear")
                            }
                        }
                    }
                )
            }

            if (isLoading) {
                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {
                    Column(
                        horizontalAlignment = Alignment.CenterHorizontally,
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        CircularProgressIndicator()
                        Text("Loading receipts...")
                    }
                }
            } else if (filteredPayments.isEmpty()) {
                Box(
                    modifier = Modifier.fillMaxSize(),
                    contentAlignment = Alignment.Center
                ) {
                    Column(
                        horizontalAlignment = Alignment.CenterHorizontally,
                        verticalArrangement = Arrangement.spacedBy(16.dp)
                    ) {
                        Icon(
                            Icons.Default.ReceiptLong,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                        Text(
                            if (payments.isEmpty()) "No receipts available" else "No matching receipts",
                            style = MaterialTheme.typography.bodyLarge,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                        if (searchQuery.isNotEmpty()) {
                            TextButton(onClick = { onSearchQueryChange("") }) {
                                Text("Clear search")
                            }
                        }
                    }
                }
            } else {
                LazyColumn(
                    modifier = Modifier.fillMaxSize(),
                    contentPadding = PaddingValues(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    items(filteredPayments, key = { it.transanctionId }) { payment ->
                        ReceiptCard(
                            payment = payment,
                            onClick = { onReceiptClick(payment) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
fun ReceiptCard(
    payment: PaymentRecord,
    onClick: () -> Unit
) {
    Card(
        onClick = onClick,
        modifier = Modifier.fillMaxWidth(),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp)
        ) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = payment.receipNumber ?: "N/A",
                    style = MaterialTheme.typography.titleMedium,
                    fontWeight = FontWeight.Bold,
                    color = MaterialTheme.colorScheme.primary
                )
                Surface(
                    shape = MaterialTheme.shapes.small,
                    color = MaterialTheme.colorScheme.primaryContainer
                ) {
                    Text(
                        text = "GHS ${String.format("%.2f", payment.paid)}",
                        modifier = Modifier.padding(horizontal = 8.dp, vertical = 4.dp),
                        style = MaterialTheme.typography.labelLarge,
                        color = MaterialTheme.colorScheme.onPrimaryContainer
                    )
                }
            }

            HorizontalDivider()

            Column(
                verticalArrangement = Arrangement.spacedBy(4.dp)
            ) {
                InfoRow(label = "Student", value = payment.studentName ?: "N/A")
                InfoRow(label = "Admission No", value = payment.admissionNo ?: "N/A")
                InfoRow(label = "Paid By", value = payment.paidBy ?: "N/A")
                InfoRow(label = "Payment Mode", value = payment.paymentMode ?: "N/A")
                InfoRow(
                    label = "Date Paid",
                    value = payment.datePaid?.take(10) ?: "N/A"
                )
                if (!payment.balance.isNaN() && payment.balance != 0.0) {
                    InfoRow(
                        label = "Balance",
                        value = "GHS ${String.format("%.2f", payment.balance)}",
                        valueColor = if (payment.balance > 0) 
                            MaterialTheme.colorScheme.error else 
                            MaterialTheme.colorScheme.primary
                    )
                }
            }

            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.End
            ) {
                TextButton(onClick = onClick) {
                    Icon(Icons.Default.Visibility, contentDescription = null, modifier = Modifier.size(18.dp))
                    Spacer(modifier = Modifier.width(4.dp))
                    Text("View Details")
                }
            }
        }
    }
}

@Composable
private fun InfoRow(
    label: String,
    value: String,
    valueColor: androidx.compose.ui.graphics.Color = MaterialTheme.colorScheme.onSurface
) {
    Row(
        modifier = Modifier.fillMaxWidth(),
        horizontalArrangement = Arrangement.SpaceBetween
    ) {
        Text(
            text = label,
            style = MaterialTheme.typography.bodySmall,
            color = MaterialTheme.colorScheme.onSurfaceVariant
        )
        Text(
            text = value,
            style = MaterialTheme.typography.bodySmall,
            fontWeight = FontWeight.Medium,
            color = valueColor,
            textAlign = TextAlign.End
        )
    }
}
