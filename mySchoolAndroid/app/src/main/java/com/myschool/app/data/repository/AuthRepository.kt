package com.myschool.app.data.repository

import com.myschool.app.data.local.AuthDataStore
import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import kotlinx.coroutines.flow.Flow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AuthRepository @Inject constructor(
    private val apiService: ApiService,
    private val authDataStore: AuthDataStore
) {
    
    suspend fun login(username: String, password: String): Result<LoginResponse> {
        return try {
            val response = apiService.login(LoginRequest(username, password))
            if (response.isSuccessful && response.body() != null) {
                val loginResponse = response.body()!!
                authDataStore.saveAuthData(
                    token = loginResponse.accessToken,
                    userId = loginResponse.username,
                    roleId = loginResponse.roleId,
                    schoolId = loginResponse.schoolId
                )
                Result.success(loginResponse)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Login failed"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun logout() {
        authDataStore.clearAuthData()
    }
    
    fun getAccessToken(): Flow<String?> = authDataStore.accessToken
    
    suspend fun isLoggedIn(): Boolean = authDataStore.isLoggedIn()
    
    suspend fun getUserId(): String? = authDataStore.userId.first()
    
    suspend fun getRoleId(): Int? = authDataStore.roleId.first()
    
    suspend fun getSchoolId(): Int? = authDataStore.schoolId.first()
}
