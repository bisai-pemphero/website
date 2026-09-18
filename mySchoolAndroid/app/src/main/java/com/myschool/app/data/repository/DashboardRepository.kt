package com.myschool.app.data.repository

import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DashboardRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    suspend fun getStats(): Result<DashboardStats> {
        return try {
            val response = apiService.getDashboardStats()
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                // Return default stats on error
                Result.success(DashboardStats())
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
