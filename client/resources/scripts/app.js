// API Base URL - Update this to match your API endpoint
const API_BASE_URL = 'http://localhost:5085/api'; // Change to your API URL

// Global state
let currentUser = null;
let categories = [];
let tasks = [];

// Initialize app
document.addEventListener('DOMContentLoaded', function() {
    // Check if user is already logged in (from localStorage)
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
        currentUser = JSON.parse(savedUser);
        showTaskSection();
        loadCategories();
        loadTasks();
    }
});

// Authentication Functions
async function handleLogin(event) {
    event.preventDefault();
    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_BASE_URL}/users/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const user = await response.json();
            currentUser = user;
            localStorage.setItem('currentUser', JSON.stringify(user));
            showTaskSection();
            loadCategories();
            loadTasks();
            document.getElementById('loginForm').reset();
        } else {
            const error = await response.json();
            alert(error.message || 'Login failed');
        }
    } catch (error) {
        console.error('Login error:', error);
        alert('Error connecting to server. Make sure the API is running.');
    }
}

async function handleRegister(event) {
    event.preventDefault();
    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;

    try {
        const response = await fetch(`${API_BASE_URL}/users/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, email, password })
        });

        if (response.ok) {
            const user = await response.json();
            currentUser = user;
            localStorage.setItem('currentUser', JSON.stringify(user));
            showTaskSection();
            loadCategories();
            loadTasks();
            document.getElementById('registerForm').reset();
            alert('Registration successful!');
        } else {
            const error = await response.json();
            alert(error.message || 'Registration failed');
        }
    } catch (error) {
        console.error('Registration error:', error);
        alert('Error connecting to server. Make sure the API is running.');
    }
}

function logout() {
    currentUser = null;
    localStorage.removeItem('currentUser');
    document.getElementById('authSection').classList.remove('d-none');
    document.getElementById('taskSection').classList.add('d-none');
    document.getElementById('userInfo').classList.add('d-none');
}

function showTaskSection() {
    document.getElementById('authSection').classList.add('d-none');
    document.getElementById('taskSection').classList.remove('d-none');
    document.getElementById('userInfo').classList.remove('d-none');
    document.getElementById('usernameDisplay').textContent = `Welcome, ${currentUser.username}`;
}

// Category Functions
async function loadCategories() {
    try {
        const response = await fetch(`${API_BASE_URL}/categories`);
        if (response.ok) {
            categories = await response.json();
            populateCategoryDropdowns();
        }
    } catch (error) {
        console.error('Error loading categories:', error);
    }
}

function populateCategoryDropdowns() {
    const categoryFilter = document.getElementById('categoryFilter');
    const taskCategory = document.getElementById('taskCategory');
    
    // Clear existing options (except "All Categories" and "None")
    categoryFilter.innerHTML = '<option value="all">All Categories</option>';
    taskCategory.innerHTML = '<option value="">None</option>';
    
    categories.forEach(category => {
        const filterOption = document.createElement('option');
        filterOption.value = category.categoryId;
        filterOption.textContent = category.categoryName;
        categoryFilter.appendChild(filterOption);
        
        const taskOption = document.createElement('option');
        taskOption.value = category.categoryId;
        taskOption.textContent = category.categoryName;
        taskCategory.appendChild(taskOption);
    });
}

// Task Functions
async function loadTasks() {
    if (!currentUser) return;

    try {
        const response = await fetch(`${API_BASE_URL}/tasks?userId=${currentUser.userId}`);
        if (response.ok) {
            tasks = await response.json();
            applyFilters();
        } else {
            console.error('Error loading tasks');
        }
    } catch (error) {
        console.error('Error loading tasks:', error);
    }
}

function applyFilters() {
    const statusFilter = document.getElementById('statusFilter').value;
    const priorityFilter = document.getElementById('priorityFilter').value;
    const categoryFilter = document.getElementById('categoryFilter').value;

    let filteredTasks = tasks;

    if (statusFilter !== 'all') {
        filteredTasks = filteredTasks.filter(task => task.status === statusFilter);
    }

    if (priorityFilter !== 'all') {
        filteredTasks = filteredTasks.filter(task => task.priority === priorityFilter);
    }

    if (categoryFilter !== 'all') {
        filteredTasks = filteredTasks.filter(task => task.categoryId == categoryFilter);
    }

    displayTasks(filteredTasks);
}

function displayTasks(taskList) {
    const container = document.getElementById('tasksContainer');
    const noTasksMessage = document.getElementById('noTasksMessage');
    
    container.innerHTML = '';

    if (taskList.length === 0) {
        noTasksMessage.classList.remove('d-none');
        return;
    }

    noTasksMessage.classList.add('d-none');

    taskList.forEach(task => {
        const category = categories.find(c => c.categoryId === task.categoryId);
        const categoryName = category ? category.categoryName : 'Uncategorized';
        const categoryColor = category ? category.color : '#6c757d';
        
        const dueDate = task.dueDate ? new Date(task.dueDate) : null;
        const isOverdue = dueDate && dueDate < new Date() && task.status !== 'Completed';
        const isCompleted = task.status === 'Completed';

        const taskCard = document.createElement('div');
        taskCard.className = `col-md-6 col-lg-4 mb-3`;
        taskCard.innerHTML = `
            <div class="card task-card priority-${task.priority.toLowerCase()} ${isCompleted ? 'completed-task' : ''}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <h5 class="card-title mb-0">${escapeHtml(task.title)}</h5>
                        <div class="dropdown">
                            <button class="btn btn-sm btn-link text-decoration-none" type="button" data-bs-toggle="dropdown">
                                <i class="bi bi-three-dots-vertical"></i>
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="#" onclick="editTask(${task.taskId}); return false;"><i class="bi bi-pencil"></i> Edit</a></li>
                                <li><a class="dropdown-item text-danger" href="#" onclick="deleteTask(${task.taskId}); return false;"><i class="bi bi-trash"></i> Delete</a></li>
                            </ul>
                        </div>
                    </div>
                    ${task.description ? `<p class="card-text text-muted small">${escapeHtml(task.description)}</p>` : ''}
                    <div class="d-flex flex-wrap gap-2 mb-2">
                        <span class="badge status-badge bg-${getStatusColor(task.status)}">${task.status}</span>
                        <span class="badge category-badge" style="background-color: ${categoryColor}">${categoryName}</span>
                        <span class="badge bg-secondary">${task.priority}</span>
                    </div>
                    ${dueDate ? `<small class="text-muted ${isOverdue ? 'text-danger' : ''}"><i class="bi bi-calendar"></i> Due: ${formatDate(dueDate)}</small>` : ''}
                    ${isCompleted && task.completedAt ? `<br><small class="text-muted"><i class="bi bi-check-circle"></i> Completed: ${formatDate(new Date(task.completedAt))}</small>` : ''}
                </div>
            </div>
        `;
        container.appendChild(taskCard);
    });
}

function getStatusColor(status) {
    const colors = {
        'Pending': 'secondary',
        'In Progress': 'primary',
        'Completed': 'success',
        'Cancelled': 'danger'
    };
    return colors[status] || 'secondary';
}

function formatDate(date) {
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Task Modal Functions
function showAddTaskModal() {
    document.getElementById('taskModalTitle').textContent = 'Add New Task';
    document.getElementById('taskForm').reset();
    document.getElementById('taskId').value = '';
    document.getElementById('taskStatus').value = 'Pending';
    document.getElementById('taskPriority').value = 'Medium';
    const modal = new bootstrap.Modal(document.getElementById('taskModal'));
    modal.show();
}

function editTask(taskId) {
    const task = tasks.find(t => t.taskId === taskId);
    if (!task) return;

    document.getElementById('taskModalTitle').textContent = 'Edit Task';
    document.getElementById('taskId').value = task.taskId;
    document.getElementById('taskTitle').value = task.title;
    document.getElementById('taskDescription').value = task.description || '';
    document.getElementById('taskCategory').value = task.categoryId || '';
    document.getElementById('taskPriority').value = task.priority;
    document.getElementById('taskStatus').value = task.status;
    
    if (task.dueDate) {
        const dueDate = new Date(task.dueDate);
        const localDateTime = new Date(dueDate.getTime() - dueDate.getTimezoneOffset() * 60000).toISOString().slice(0, 16);
        document.getElementById('taskDueDate').value = localDateTime;
    } else {
        document.getElementById('taskDueDate').value = '';
    }

    const modal = new bootstrap.Modal(document.getElementById('taskModal'));
    modal.show();
}

async function handleTaskSubmit(event) {
    event.preventDefault();
    
    const taskId = document.getElementById('taskId').value;
    const taskData = {
        userId: currentUser.userId,
        title: document.getElementById('taskTitle').value,
        description: document.getElementById('taskDescription').value,
        categoryId: document.getElementById('taskCategory').value ? parseInt(document.getElementById('taskCategory').value) : null,
        priority: document.getElementById('taskPriority').value,
        status: document.getElementById('taskStatus').value,
        dueDate: document.getElementById('taskDueDate').value ? new Date(document.getElementById('taskDueDate').value).toISOString() : null
    };

    try {
        let response;
        if (taskId) {
            // Update existing task
            taskData.taskId = parseInt(taskId);
            response = await fetch(`${API_BASE_URL}/tasks/${taskId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(taskData)
            });
        } else {
            // Create new task
            response = await fetch(`${API_BASE_URL}/tasks`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(taskData)
            });
        }

        if (response.ok) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('taskModal'));
            modal.hide();
            loadTasks();
        } else {
            const error = await response.json();
            alert(error.message || 'Error saving task');
        }
    } catch (error) {
        console.error('Error saving task:', error);
        alert('Error connecting to server');
    }
}

async function deleteTask(taskId) {
    if (!confirm('Are you sure you want to delete this task?')) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/tasks/${taskId}?userId=${currentUser.userId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            loadTasks();
        } else {
            alert('Error deleting task');
        }
    } catch (error) {
        console.error('Error deleting task:', error);
        alert('Error connecting to server');
    }
}

