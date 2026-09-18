package com.myschool.app.data.repository

import com.myschool.app.data.remote.ApiService
import com.myschool.app.domain.model.*
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AcademicRepository @Inject constructor(
    private val apiService: ApiService
) {
    
    // Academic Years
    suspend fun getAcademicYears(page: Int = 1, pageSize: Int = 50): Result<List<AcademicYear>> {
        return try {
            val response = apiService.getAcademicYears(page, pageSize)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch academic years"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createAcademicYear(year: AcademicYear): Result<AcademicYear> {
        return try {
            val response = apiService.createAcademicYear(year)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create academic year"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateAcademicYear(yearId: Int, year: AcademicYear): Result<AcademicYear> {
        return try {
            val response = apiService.updateAcademicYear(yearId, year)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update academic year"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteAcademicYear(yearId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteAcademicYear(yearId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete academic year"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Terms
    suspend fun getTerms(page: Int = 1, pageSize: Int = 50, academicYearId: Int? = null): Result<List<AcademicTerm>> {
        return try {
            val response = apiService.getTerms(page, pageSize, academicYearId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch terms"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createTerm(term: AcademicTerm): Result<AcademicTerm> {
        return try {
            val response = apiService.createTerm(term)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create term"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateTerm(termId: Int, term: AcademicTerm): Result<AcademicTerm> {
        return try {
            val response = apiService.updateTerm(termId, term)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update term"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteTerm(termId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteTerm(termId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete term"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Classes
    suspend fun getClasses(page: Int = 1, pageSize: Int = 50, academicYearId: Int? = null): Result<List<ClassGroup>> {
        return try {
            val response = apiService.getClasses(page, pageSize, academicYearId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch classes"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createClass(classGroup: ClassGroup): Result<ClassGroup> {
        return try {
            val response = apiService.createClass(classGroup)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create class"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateClass(classId: Int, classGroup: ClassGroup): Result<ClassGroup> {
        return try {
            val response = apiService.updateClass(classId, classGroup)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update class"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteClass(classId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteClass(classId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete class"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Subjects
    suspend fun getSubjects(page: Int = 1, pageSize: Int = 50, classId: Int? = null): Result<List<Subject>> {
        return try {
            val response = apiService.getSubjects(page, pageSize, classId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch subjects"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createSubject(subject: Subject): Result<Subject> {
        return try {
            val response = apiService.createSubject(subject)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create subject"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateSubject(subjectId: Int, subject: Subject): Result<Subject> {
        return try {
            val response = apiService.updateSubject(subjectId, subject)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update subject"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteSubject(subjectId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteSubject(subjectId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete subject"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    // Exams
    suspend fun getExams(page: Int = 1, pageSize: Int = 50, academicYearId: Int? = null, termId: Int? = null): Result<List<Exam>> {
        return try {
            val response = apiService.getExams(page, pageSize, academicYearId, termId)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to fetch exams"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun createExam(exam: Exam): Result<Exam> {
        return try {
            val response = apiService.createExam(exam)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to create exam"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun updateExam(examId: Int, exam: Exam): Result<Exam> {
        return try {
            val response = apiService.updateExam(examId, exam)
            if (response.isSuccessful && response.body() != null) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to update exam"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun deleteExam(examId: Int): Result<Unit> {
        return try {
            val response = apiService.deleteExam(examId)
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception(response.errorBody()?.string() ?: "Failed to delete exam"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
