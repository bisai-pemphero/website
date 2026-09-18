package com.myschool.app.data.repository

import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class StudentRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    suspend fun getStudents(
        page: Int = 1,
        pageSize: Int = 50,
        search: String? = null,
        classId: Int? = null,
        academicYearId: Int? = null
    ): Result<List<Student>> {
        return try {
            val response = apiService.getStudents(page, pageSize, search, classId, academicYearId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch students"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getStudent(studentId: Int): Result<Student> {
        return try {
            val response = apiService.getStudent(studentId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch student"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createStudent(student: Student): Result<Student> {
        return try {
            val response = apiService.createStudent(student)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create student"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateStudent(studentId: Int, student: Student): Result<Student> {
        return try {
            val response = apiService.updateStudent(studentId, student)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update student"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteStudent(studentId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteStudent(studentId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete student"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getStudentBalance(studentId: Int): Result<Map<String, Any>> {
        return try {
            val response = apiService.getStudentBalance(studentId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch balance"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
