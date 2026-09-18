package com.myschool.app.data.repository

import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class GradingRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    suspend fun getGradingSystems(level: String? = null): Result<List<Grade>> {
        return try {
            val response = apiService.getGradingSystems(level)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch grading systems"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getGradingSystem(id: Int): Result<Grade> {
        return try {
            val response = apiService.getGradingSystem(id)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch grading system"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createGradingSystem(grade: Grade): Result<Grade> {
        return try {
            val response = apiService.createGradingSystem(grade)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create grading system"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateGradingSystem(id: Int, grade: Grade): Result<Grade> {
        return try {
            val response = apiService.updateGradingSystem(id, grade)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update grading system"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteGradingSystem(id: Int): Result<Unit> {
        return try {
            val response = apiService.deleteGradingSystem(id)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete grading system"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}

@Singleton
class TeacherRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    suspend fun getTeacherAssignments(page: Int = 1, pageSize: Int = 50, teacherId: Int? = null, classId: Int? = null): Result<List<TeacherAssignment>> {
        return try {
            val response = apiService.getTeacherAssignments(page, pageSize, teacherId, classId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch teacher assignments"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createTeacherAssignment(assignment: TeacherAssignment): Result<TeacherAssignment> {
        return try {
            val response = apiService.createTeacherAssignment(assignment)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create teacher assignment"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateTeacherAssignment(assignmentId: Int, assignment: TeacherAssignment): Result<TeacherAssignment> {
        return try {
            val response = apiService.updateTeacherAssignment(assignmentId, assignment)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update teacher assignment"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteTeacherAssignment(assignmentId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteTeacherAssignment(assignmentId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete teacher assignment"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Class Teachers
    suspend fun getClassTeachers(page: Int = 1, pageSize: Int = 50): Result<List<ClassTeacher>> {
        return try {
            val response = apiService.getClassTeachers(page, pageSize)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch class teachers"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createClassTeacher(classTeacher: ClassTeacher): Result<ClassTeacher> {
        return try {
            val response = apiService.createClassTeacher(classTeacher)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create class teacher"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateClassTeacher(id: Int, classTeacher: ClassTeacher): Result<ClassTeacher> {
        return try {
            val response = apiService.updateClassTeacher(id, classTeacher)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update class teacher"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteClassTeacher(id: Int): Result<Unit> {
        return try {
            val response = apiService.deleteClassTeacher(id)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete class teacher"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Student Promotion
    suspend fun getPromotionClasses(): Result<List<PromotionClass>> {
        return try {
            val response = apiService.getPromotionClasses()
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch promotion classes"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun getPromotionStudents(classId: Int?): Result<List<Student>> {
        return try {
            val response = apiService.getPromotionStudents(classId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch promotion students"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun promoteStudents(request: PromotionRequest): Result<Map<String, Any>> {
        return try {
            val response = apiService.promoteStudents(request)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to promote students"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun bulkChangeClass(request: ChangeClassRequest): Result<Map<String, Any>> {
        return try {
            val response = apiService.bulkChangeClass(request)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to change class"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
