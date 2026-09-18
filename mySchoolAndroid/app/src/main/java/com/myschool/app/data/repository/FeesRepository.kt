package com.myschool.app.data.repository

import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class FeesRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    // Fees
    suspend fun getFees(page: Int = 1, pageSize: Int = 50, classId: Int? = null, academicYearId: Int? = null): Result<List<Fee>> {
        return try {
            val response = apiService.getFees(page, pageSize, classId, academicYearId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch fees"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createFee(fee: Fee): Result<Fee> {
        return try {
            val response = apiService.createFee(fee)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create fee"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateFee(feeId: Int, fee: Fee): Result<Fee> {
        return try {
            val response = apiService.updateFee(feeId, fee)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update fee"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteFee(feeId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteFee(feeId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete fee"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Payments
    suspend fun getPayments(page: Int = 1, pageSize: Int = 50, studentId: Int? = null, startDate: String? = null, endDate: String? = null): Result<List<FeePayment>> {
        return try {
            val response = apiService.getPayments(page, pageSize, studentId, startDate, endDate)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch payments"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createPayment(payment: FeePayment): Result<FeePayment> {
        return try {
            val response = apiService.createPayment(payment)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create payment"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getPayment(paymentId: Int): Result<FeePayment> {
        return try {
            val response = apiService.getPayment(paymentId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch payment"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Receipts
    suspend fun getReceipts(page: Int = 1, pageSize: Int = 50, studentId: Int? = null): Result<List<Receipt>> {
        return try {
            val response = apiService.getReceipts(page, pageSize, studentId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch receipts"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getReceipt(receiptId: Int): Result<Receipt> {
        return try {
            val response = apiService.getReceipt(receiptId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch receipt"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun reprintReceipt(receiptId: Int): Result<Receipt> {
        return try {
            val response = apiService.reprintReceipt(receiptId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to reprint receipt"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Student Balances
    suspend fun getOutstandingBalances(classId: Int? = null): Result<List<StudentBalance>> {
        return try {
            val response = apiService.getOutstandingBalancesReport(classId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch outstanding balances"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
