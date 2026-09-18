package com.myschool.app.domain.model

data class LoginRequest(
    val username: String,
    val password: String
)

data class LoginResponse(
    val accessToken: String,
    val username: String,
    val roleId: Int,
    val schoolId: Int
)

data class UserProfile(
    val username: String,
    val roleId: Int,
    val roleName: String,
    val schoolId: Int,
    val schoolName: String? = null
) {
    fun isSystemAdmin(): Boolean = roleId == 1
    fun isDirector(): Boolean = roleId == 2
    fun isHeadmaster(): Boolean = roleId == 3
    fun isBursar(): Boolean = roleId == 4
    fun isTeacher(): Boolean = roleId == 5
}

data class School(
    val schoolId: Int,
    val schoolName: String,
    val schoolCode: String?,
    val address: String?,
    val phone: String?,
    val email: String?,
    val isActive: Boolean = true,
    val createdAt: String? = null
)

data class User(
    val userId: Int,
    val username: String,
    val fullName: String,
    val email: String?,
    val phone: String?,
    val roleId: Int,
    val roleName: String?,
    val schoolId: Int,
    val schoolName: String?,
    val isActive: Boolean = true,
    val createdAt: String? = null
)

data class Student(
    val studentId: Int,
    val admissionNumber: String,
    val firstName: String,
    val lastName: String,
    val middleName: String? = null,
    val dateOfBirth: String? = null,
    val gender: String? = null,
    val nationality: String? = null,
    val state: String? = null,
    val city: String? = null,
    val address: String? = null,
    val phone: String? = null,
    val email: String? = null,
    val classId: Int? = null,
    val className: String? = null,
    val academicYearId: Int? = null,
    val isActive: Boolean = true,
    val schoolId: Int,
    val createdAt: String? = null,
    val guardianName: String? = null,
    val guardianPhone: String? = null,
    val guardianEmail: String? = null,
    val balance: Double = 0.0
)

data class AcademicYear(
    val academicYearId: Int,
    val yearName: String,
    val startDate: String,
    val endDate: String,
    val isActive: Boolean = false,
    val schoolId: Int,
    val createdAt: String? = null
)

data class AcademicTerm(
    val termId: Int,
    val termName: String,
    val academicYearId: Int,
    val startDate: String? = null,
    val endDate: String? = null,
    val isActive: Boolean = false,
    val schoolId: Int,
    val createdAt: String? = null
)

data class ClassGroup(
    val classId: Int,
    val className: String,
    val section: String? = null,
    val academicYearId: Int? = null,
    val schoolId: Int,
    val isActive: Boolean = true
)

data class Subject(
    val subjectId: Int,
    val subjectName: String,
    val subjectCode: String? = null,
    val classId: Int? = null,
    val schoolId: Int,
    val isActive: Boolean = true
)

data class Exam(
    val examId: Int,
    val examName: String,
    val examType: String? = null,
    val academicYearId: Int? = null,
    val termId: Int? = null,
    val startDate: String? = null,
    val endDate: String? = null,
    val isActive: Boolean = true,
    val schoolId: Int
)

data class Fee(
    val feeId: Int,
    val feeName: String,
    val feeAmount: Double,
    val feeType: String? = null,
    val classId: Int? = null,
    val academicYearId: Int? = null,
    val termId: Int? = null,
    val description: String? = null,
    val isActive: Boolean = true,
    val schoolId: Int,
    val createdAt: String? = null
)

data class FeePayment(
    val paymentId: Int,
    val studentId: Int,
    val studentName: String? = null,
    val admissionNumber: String? = null,
    val amount: Double,
    val paymentDate: String,
    val paymentMethod: String? = null,
    val referenceNumber: String? = null,
    val description: String? = null,
    val paidBy: String? = null,
    val schoolId: Int,
    val receiptId: Int? = null
)

data class Receipt(
    val receiptId: Int,
    val receiptNumber: String,
    val studentId: Int,
    val studentName: String? = null,
    val admissionNumber: String? = null,
    val amount: Double,
    val paymentDate: String,
    val paymentMethod: String? = null,
    val paidBy: String? = null,
    val description: String? = null,
    val schoolId: Int,
    val issuedBy: String? = null,
    val createdAt: String? = null
)

data class Grade(
    val gradeId: Int,
    val gradeName: String,
    val minMarks: Double,
    val maxMarks: Double,
    val points: Double? = null,
    val remark: String? = null,
    val schoolId: Int,
    val isActive: Boolean = true
)

data class TeacherAssignment(
    val assignmentId: Int,
    val teacherId: Int,
    val teacherName: String? = null,
    val subjectId: Int,
    val subjectName: String? = null,
    val classId: Int,
    val className: String? = null,
    val academicYearId: Int? = null,
    val schoolId: Int
)

data class ClassTeacher(
    val id: Int,
    val teacherId: Int,
    val teacherName: String? = null,
    val classId: Int,
    val className: String? = null,
    val schoolId: Int
)

data class PromotionClass(
    val classId: Int,
    val className: String,
    val isGraduate: Boolean = false
)

data class PromotionRequest(
    val studentIds: List<Int>,
    val targetClassId: Int,
    val academicYearId: Int? = null,
    val isGraduating: Boolean = false
)

data class ChangeClassRequest(
    val studentIds: List<Int>,
    val newClassId: Int
)

data class StudentBalance(
    val studentId: Int,
    val studentName: String,
    val admissionNumber: String,
    val className: String?,
    val totalFees: Double,
    val amountPaid: Double,
    val balance: Double
)

data class DashboardStats(
    val totalStudents: Int = 0,
    val totalTeachers: Int = 0,
    val totalClasses: Int = 0,
    val totalFeesCollected: Double = 0.0,
    val outstandingFees: Double = 0.0,
    val activeAcademicYear: String? = null
)

data class ApiError(
    val message: String,
    val code: String? = null,
    val details: Map<String, String>? = null
)
